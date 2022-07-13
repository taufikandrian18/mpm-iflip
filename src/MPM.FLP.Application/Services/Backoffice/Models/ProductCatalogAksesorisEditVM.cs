using System;
namespace MPM.FLP.Services.Backoffice
{
    public class ProductCatalogAksesorisEditVM
    {
        public Guid ProductCatalogId { get; set; }
        public Guid guid { get; set; }
        public string Title { get; set; }
        public int PanjangLebarTinggi { get; set; }
        public string DiameterLangkah { get; set; }
        public string ProductCode { get; set; }
    }
}
