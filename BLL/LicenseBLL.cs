using GenerateEncryptedFile.Models;

namespace GenerateEncryptedFile.BLL
{
    public class LicenseBLL : ILicenseBLL
    {
        public List<LicenseModel> GetAllComponents()
        {

            string []componentsArr = { "E-Commerce", "FinTech", "Transportation"};

            List<LicenseModel> licenseModels = new List<LicenseModel>();

            for (int i = 0; i < componentsArr.Length; i++)
            {
                licenseModels.Add(new LicenseModel()
                {
                    Id = i + 1,
                    ComponentName = componentsArr[i]
                });
            }

            return licenseModels;
        }

        public List<LicenseModel> GetRequestedComponentsIds(List<int> componentIds)
        {
            var components = GetAllComponents();
            List<LicenseModel> ret = new List<LicenseModel>();

            foreach (var id in componentIds)
            {
                foreach (var component in components)
                {
                    if (id == component.Id)
                        ret.Add(component);
                }
            }

            return ret;
        }
    }
}
