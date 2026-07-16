using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class SslCommerzConfig
{
    public string StoreId { get; set; } = string.Empty;
    public string StorePassword { get; set; } = string.Empty;
    public bool IsSandbox { get; set; } = true;
    public string Currency { get; set; } = "BDT";
    public string SuccessUrl { get; set; } = string.Empty;
    public string FailUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string IpnUrl { get; set; } = string.Empty;
    public string InitUrl { get; set; } = string.Empty;
    public string ValidationUrl { get; set; } = string.Empty;
}

public class SslCommerzInitRequest
{
    public string store_id { get; set; } = string.Empty;
    public string store_passwd { get; set; } = string.Empty;
    public decimal total_amount { get; set; }
    public string currency { get; set; } = "BDT";
    public string tran_id { get; set; } = string.Empty;
    public string success_url { get; set; } = string.Empty;
    public string fail_url { get; set; } = string.Empty;
    public string cancel_url { get; set; } = string.Empty;
    public string ipn_url { get; set; } = string.Empty;
    public string cus_name { get; set; } = string.Empty;
    public string cus_email { get; set; } = string.Empty;
    public string cus_phone { get; set; } = string.Empty;
    public string cus_add1 { get; set; } = string.Empty;
    public string cus_city { get; set; } = string.Empty;
    public string cus_country { get; set; } = "Bangladesh";
    public string product_name { get; set; } = string.Empty;
    public string product_category { get; set; } = "Fee Payment";
    public string product_profile { get; set; } = "general";
    public string? value_a { get; set; }
    public string? value_b { get; set; }
    public string? value_c { get; set; }
    public string? value_d { get; set; }

    [MaxLength(50)]
    public string? card_type { get; set; }
}

public class SslCommerzInitResponse
{
    public string status { get; set; } = string.Empty;
    public string? failedreason { get; set; }
    public string? sessionkey { get; set; }
    public string? GatewayPageURL { get; set; }
    public string? store_amount { get; set; }
    public string? tran_id { get; set; }
    public string? bank_tran_id { get; set; }
}

public class SslCommerzValidationRequest
{
    public string? val_id { get; set; }
    public string? store_id { get; set; }
    public string? store_passwd { get; set; }
    public string? format { get; set; } = "json";
    public string? v { get; set; } = "1";
}

public class SslCommerzValidationResponse
{
    public string status { get; set; } = string.Empty;
    public string? tran_date { get; set; }
    public string? tran_id { get; set; }
    public string? val_id { get; set; }
    public decimal? amount { get; set; }
    public string? store_amount { get; set; }
    public string? bank_tran_id { get; set; }
    public string? card_type { get; set; }
    public string? currency { get; set; }
    public string? risk_title { get; set; }
    public string? risk_level { get; set; }
    public string? APIConnect { get; set; }
    public string? validated_on { get; set; }
    public string? gw_version { get; set; }
}
