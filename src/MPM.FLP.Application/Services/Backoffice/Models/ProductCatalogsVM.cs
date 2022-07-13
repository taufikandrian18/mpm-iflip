using System;
namespace MPM.FLP.Services.Backoffice
{
    public class ProductCatalogsVM
    {
        public Guid Id { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public bool IsPublished { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string SparepartDocUrl { get; set; }
        public string PanjangLebarTinggi { get; set; }
        public string JarakSumbuRoda  { get; set; }
        public string JarakTerendahkeTanah    { get; set; }
        public string BeratKosong { get; set; }
        public string KapasitasTangkiBahanBakar   { get; set; }
        public string TipeMesin   { get; set; }
        public string VolumeLangkah   { get; set; }
        public string SistemPendingin { get; set; }
        public string SistemSuplaiBahanBakar  { get; set; }
        public string DiameterLangkah { get; set; }
        public string TipeTransmisi   { get; set; }
        public string PerbandinganKompresi    { get; set; }
        public string DayaMaksimum    { get; set; }
        public string TorsiMaksimum   { get; set; }
        public string PolaPengoperanGigi  { get; set; }
        public string TipeStarter { get; set; }
        public string TipeKopling { get; set; }
        public string KapasitasMinyakPelumas  { get; set; }
        public string TipeRangka  { get; set; }
        public string UkuranBanDepan  { get; set; }
        public string UkuranBanBelakang   { get; set; }
        public string TipeRemDepan    { get; set; }
        public string TipeRemBelakang { get; set; }
        public string TipeSuspensiDepan   { get; set; }
        public string TipeSuspensiBelakang    { get; set; }
        public string TipeBaterai { get; set; }
        public string SistemPengapian { get; set; }
        public string TipeBateraiAki  { get; set; }
        public string TipeBusi    { get; set; }
        public string ProductCode { get; set; }
    }
}
