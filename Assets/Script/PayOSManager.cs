using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System.Security.Cryptography;
using System.Text;

public class PayOSManager : MonoBehaviour
{
    [Header("MÃ API PAYOS (LẤY TRÊN WEB)")]
    public string clientId;
    public string apiKey;
    public string checksumKey;

    [Header("GIAO DIỆN THANH TOÁN")]
    public GameObject paymentPanel;
    public Transform mainContainer;
    public CanvasGroup backgroundFade;
    public RawImage qrRawImage;
    public TextMeshProUGUI infoDisplayText;
    public TextMeshProUGUI contentDisplayText;
    public TextMeshProUGUI statusText;

    [Header("THÔNG TIN TÀI KHOẢN KHÁCH")]
    public string myBankId = "MB"; 
    public string myAccountNo = "8056898789"; 
    public string myAccountName = "LE TRAN XUAN NHI";

    private long currentOrderCode;
    private Sequence paymentSeq;

    private int pendingGems;
    private int pendingAmount;

    void Start() { paymentPanel.SetActive(false); }

    public void OpenPayment(int amount, string packName, string itemCode, int gems)
    {
        pendingGems = gems;
        pendingAmount = amount; 

        if (paymentSeq != null) paymentSeq.Kill();
        paymentPanel.SetActive(true);
        mainContainer.localScale = Vector3.zero;
        if (backgroundFade != null) backgroundFade.alpha = 0;

        paymentSeq = DOTween.Sequence().SetUpdate(true);
        paymentSeq.Join(backgroundFade.DOFade(1, 0.3f));
        paymentSeq.Join(mainContainer.DOScale(1, 0.4f).SetEase(Ease.OutBack));

        currentOrderCode = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        statusText.text = "ĐANG TẠO ĐƠN HÀNG...";
        
        qrRawImage.texture = null;
        qrRawImage.color = new Color(1, 1, 1, 0f); 

        StartCoroutine(CreatePayOSOrder(amount, itemCode, currentOrderCode));
    }

    IEnumerator CreatePayOSOrder(int amount, string itemCode, long orderCode)
    {
        string url = "https://api-merchant.payos.vn/v2/payment-requests";
        string safeDesc = "DracoValley_" + itemCode.Replace(" ", "");

        infoDisplayText.text = $"Ngân hàng: {myBankId}\nSTK: {myAccountNo}\nChủ TK: {myAccountName}\nSố tiền: {amount:N0} VNĐ";
        contentDisplayText.text = "Nội dung: " + safeDesc;

        string sigData = $"amount={amount}&cancelUrl=https://google.com&description={safeDesc}&orderCode={orderCode}&returnUrl=https://google.com";
        string signature = GenerateHMAC(sigData, checksumKey);

        string jsonBody = "{\"orderCode\":" + orderCode + ",\"amount\":" + amount + ",\"description\":\"" + safeDesc + "\",\"cancelUrl\":\"https://google.com\",\"returnUrl\":\"https://google.com\",\"signature\":\"" + signature + "\"}";

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("x-client-id", clientId);
        www.SetRequestHeader("x-api-key", apiKey);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string qrCodeStr = ExtractValue(www.downloadHandler.text, "qrCode");
            
            if (!string.IsNullOrEmpty(qrCodeStr))
            {
                string qrUrl = "https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=" + UnityWebRequest.EscapeURL(qrCodeStr);
                
                StartCoroutine(DownloadQR(qrUrl));
                statusText.text = "CHỜ THANH TOÁN...";
            }
            else
            {
                statusText.text = "<color=red>KHÔNG TÌM THẤY MÃ QR!</color>";
            }
        }
        else 
        { 
            Debug.LogError("Error PayOS: " + www.downloadHandler.text);
            statusText.text = "<color=red>LỖI TẠO ĐƠN!</color>"; 
        }
    }

    public void CheckPaymentStatus()
    {
        StartCoroutine(VerifyPayment());
    }

    IEnumerator VerifyPayment()
    {
        statusText.text = "ĐANG KIỂM TRA...";
        string url = "https://api-merchant.payos.vn/v2/payment-requests/" + currentOrderCode;
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("x-client-id", clientId);
        www.SetRequestHeader("x-api-key", apiKey);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text.Contains("\"status\":\"PAID\""))
        {
            statusText.text = "<color=green>THÀNH CÔNG!</color>";

            GemManager.Instance.AddGems(pendingGems);
                
         
            if (PlayFabManager.Instance != null)
            {
                PlayFabManager.Instance.SaveTransactionToPlayFab(currentOrderCode.ToString(), pendingAmount, pendingGems);
            }

            Invoke("CloseUI", 1.5f);
        }
        else 
        { 
            statusText.text = "<color=red>CHƯA NHẬN ĐƯỢC TIỀN!</color>"; 
        }
    }

    public void CloseUI()
    {
        paymentSeq = DOTween.Sequence().SetUpdate(true);
        paymentSeq.Join(mainContainer.DOScale(0, 0.3f).SetEase(Ease.InBack));
        paymentSeq.Join(backgroundFade.DOFade(0, 0.2f));
        paymentSeq.OnComplete(() => paymentPanel.SetActive(false));
    }

    IEnumerator DownloadQR(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success) 
        {
            qrRawImage.texture = DownloadHandlerTexture.GetContent(www);
            
            qrRawImage.color = new Color(1, 1, 1, 1f); 
        }
        else
        {
            Debug.LogError("Tải mã QR thất bại: " + www.error);
            statusText.text = "<color=red>LỖI KẾT NỐI TẢI QR!</color>";
        }
    }

    string GenerateHMAC(string data, string key)
    {
        byte[] keyByte = Encoding.UTF8.GetBytes(key);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    string ExtractValue(string json, string key)
    {
        try
        {
            int startIndex = json.IndexOf(key) + key.Length + 3;
            int endIndex = json.IndexOf("\"", startIndex);
            if (startIndex > 0 && endIndex > startIndex)
            {
                return json.Substring(startIndex, endIndex - startIndex);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi tách chuỗi QR: " + e.Message);
        }
        return "";
    }
}