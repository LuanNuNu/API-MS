namespace MenShop_Assignment.Extensions
{
	public enum PaymentMethod
	{
		Cash,               // Tiền mặt tại cửa hàng
		Momo,            // Ví điện tử (MoMo, ZaloPay, VNPay...)
		COD,
		VNPay,

		CreditCard,         // Thẻ tín dụng
		DebitCard,          // Thẻ ghi nợ
		BankTransfer,       // Chuyển khoản ngân hàng
		StoreCredit         // Điểm thưởng/credit của cửa hàng
	}
}
