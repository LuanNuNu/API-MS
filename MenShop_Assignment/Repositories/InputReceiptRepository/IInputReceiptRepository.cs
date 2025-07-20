using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.InputReceiptRepository
{
	public interface IInputReceiptRepository
	{
		Task<bool> CancelReceipt(int Id);
		Task<bool> ConfirmReceipt(int Id);
		Task<bool> CreateInputReceipt(List<CreateReceiptDetailDTO> detailDTOs, string ManagerId);
		Task<InputReceiptViewModel?> GetByIdAsync(int Id);
		Task<List<InputReceiptViewModel>?> GetInputReceipts();
	}
}