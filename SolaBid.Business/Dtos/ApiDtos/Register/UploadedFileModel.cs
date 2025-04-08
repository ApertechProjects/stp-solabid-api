using SolaBid.Business.Dtos.ApiDtos.Register;
using System.Collections.Generic;

public class UploadedFileModel
{
    public UploadedFileModel()
    {
        Files = new List<RegisterAttachmentCreateDto>();
    }
    public List<RegisterAttachmentCreateDto> Files { get; set; }
    public string FolderName { get; set; }
}