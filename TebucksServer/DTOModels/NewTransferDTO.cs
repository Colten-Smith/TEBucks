using System.ComponentModel.DataAnnotations;
using TEBucksServer.Models;

namespace TEbucksServer.DTOModels
{
    public class NewTransferDTO
    {
        [Range(1000, int.MaxValue)]
        public int UserFrom { get; set; }

        [Range(1000, int.MaxValue)]
        public int UserTo { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Amount { get; set; }
        public string TransferType { get; set; }
    }
}
