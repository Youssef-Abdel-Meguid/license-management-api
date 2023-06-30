using GenerateEncryptedFile.Models;

namespace GenerateEncryptedFile.BLL
{
    public interface ILicenseBLL
    {
        public List<LicenseModel> GetAllComponents();
        public List<LicenseModel> GetRequestedComponentsIds(List<int> componentIds);
    }
}
