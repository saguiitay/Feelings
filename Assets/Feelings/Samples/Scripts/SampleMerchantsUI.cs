using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SampleMerchantsUI : MonoBehaviour
{
    public HonestMerchangeMap merchant1 = new HonestMerchangeMap();
    public DishonestMerchangeMap merchant2 = new DishonestMerchangeMap();

    public float product1BasePrice = 10.0f;
    public float product2BasePrice = 20.0f;
    public float product3BasePrice = 50.0f;
    public float product4BasePrice = 100.0f;

    public Text merchant1Product1;
    public Text merchant1Product2;
    public Text merchant1Product3;
    public Text merchant1Product4;

    public Text merchant2Product1;
    public Text merchant2Product2;
    public Text merchant2Product3;
    public Text merchant2Product4;

    void Start()
    {
        UpdatePrices();
    }

    void UpdatePrices()
	{
	    merchant1Product1.text = string.Format("{0:C}", merchant1.CalcuatePrice(product1BasePrice));
	    merchant1Product2.text = string.Format("{0:C}", merchant1.CalcuatePrice(product2BasePrice));
	    merchant1Product3.text = string.Format("{0:C}", merchant1.CalcuatePrice(product3BasePrice));
	    merchant1Product4.text = string.Format("{0:C}", merchant1.CalcuatePrice(product4BasePrice));
	    merchant2Product1.text = string.Format("{0:C}", merchant2.CalcuatePrice(product1BasePrice));
	    merchant2Product2.text = string.Format("{0:C}", merchant2.CalcuatePrice(product2BasePrice));
        merchant2Product3.text = string.Format("{0:C}", merchant2.CalcuatePrice(product3BasePrice));
	    merchant2Product4.text = string.Format("{0:C}", merchant2.CalcuatePrice(product4BasePrice));
	}

    public void BuySomething()
    {
        merchant1.BuySomething();
        merchant2.BuySomething();
        UpdatePrices();
    }

    public void Flatter()
    {
        merchant1.Flatter();
        merchant2.Flatter();
        UpdatePrices();
    }

    public void Threaten()
    {
        merchant1.Threaten();
        merchant2.Threaten();
        UpdatePrices();
    }

    public void Bribe()
    {
        merchant1.Bribe();
        merchant2.Bribe();
        UpdatePrices();
    }
}