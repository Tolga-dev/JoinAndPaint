using System;
using UnityEngine;
using UnityEngine.Events;

namespace Controller
{
    [Serializable]
    public class InAppPurchaseController
    {
        public string ReturnLocalizedPrice(string id)
        {
            /*var product = GetItem(id);
            if (product != null)
            {
                decimal price = product.metadata.localizedPrice;
                string code = product.metadata.isoCurrencyCode;
                return price + " " + code;
            }*/
            return "N/A";
        }

        public void BuyItem(string noAdsProductId, UnityAction<string> success, UnityAction<string> failed)
        {
            Debug.Log("Buy it!");
            success.Invoke("Success");
        }
    }
}