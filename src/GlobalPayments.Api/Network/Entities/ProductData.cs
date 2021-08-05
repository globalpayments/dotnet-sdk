using GlobalPayments.Api.Network.Elements;
using GlobalPayments.Api.Utils;
using System.Linq;

namespace GlobalPayments.Api.Network.Entities {
    public class ProductData {
        private DE63_ProductData productData;

        public ProductData(ServiceLevel serviceLevel, ProductCodeSet productCodeSet = ProductCodeSet.Heartland) {
            productData = new DE63_ProductData {
                ProductDataFormat = ProductDataFormat.HeartlandStandardFormat,
                ProductCodeSet = productCodeSet,
                ServiceLevel = serviceLevel
            };
        }

        public void Add(ProductCode productCode, UnitOfMeasure unitOfMeasure, double quantity, double price) {
            Add(productCode, unitOfMeasure, new decimal(quantity), new decimal(price), new decimal(quantity * price));
        }

        public void Add(ProductCode productCode, UnitOfMeasure unitOfMeasure, decimal quantity, decimal price, decimal amount) {
            Add(EnumConverter.GetMapping(Target.NWS, productCode), unitOfMeasure, quantity, price, amount);
        }

        public void Add(string productCode, UnitOfMeasure unitOfMeasure, double quantity, double price) {
            Add(productCode, unitOfMeasure, new decimal(quantity), new decimal(price), new decimal(quantity * price));
        }

        public void Add(string productCode, UnitOfMeasure unitOfMeasure, decimal quantity, decimal price, decimal amount) {
            DE63_ProductDataEntry entry = new DE63_ProductDataEntry {
                Code = productCode,
                UnitOfMeasure = unitOfMeasure,
                Quantity = quantity,
                Price = price,
                Amount = amount
            };
            productData.Add(entry);
        }
        public int Count { get { return productData.ProductCount; } }

        public DE63_ProductData ToDataElement() {
            return productData;
        }
    }
}
