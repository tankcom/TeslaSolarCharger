﻿namespace TeslaSolarCharger.Model.Entities.TeslaSolarCharger;

public class ChargePrice
{
    public int Id { get; set; }
    public DateTime ValidSince { get; set; }
    public decimal SolarPrice { get; set; }
    public decimal GridPrice { get; set; }
    public bool AddSpotPriceToGridPrice { get; set; }
    public decimal SpotPriceCorrectionFactor { get; set; }
}
