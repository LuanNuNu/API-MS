using MenShop_Assignment.DTOs;
using MenShop_Assignment.Models;

namespace MenShop_Assignment.Repositories.OutputReceiptRepositories
{
	public interface IOutputReceiptRepository
	{
		Task<bool> CancelReceipt(int Id);
		Task<bool> ConfirmReceipt(int Id);
		Task<bool> CreateReceipt(int branchId, string managerId, List<CreateReceiptDetailDTO> detailDTOs);
		Task<List<OutputReceiptViewModel>> GetByBranchId(int BranchId);
		Task<OutputReceiptViewModel> GetById(int Id);
		Task<List<OutputReceiptViewModel>> GetOutputReceiptViews();
	}
}