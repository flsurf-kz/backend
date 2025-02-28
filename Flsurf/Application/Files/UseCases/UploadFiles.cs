
using Flsurf.Application.Common.UseCases;
using Flsurf.Application.Files.Dto;
using Flsurf.Domain.Files.Entities;

namespace Flsurf.Application.Files.UseCases
{
    public class UploadFiles : BaseUseCase<ICollection<CreateFileDto>, ICollection<FileEntity>>
    {
        private readonly UploadFile _uploadFile;

        public UploadFiles(UploadFile uploadFile)
        {
            _uploadFile = uploadFile;
        }

        public async Task<ICollection<FileEntity>> Execute(ICollection<CreateFileDto> files)
        {
            ICollection<FileEntity> newFiles = [];
            foreach (var file in files)
            {
                // довольно костыльное решение, но сойдет ¯\_(ツ)_/¯
                var newFile = await _uploadFile.Execute(file);

                newFiles.Add(newFile);
            }

            return newFiles;
        }
    }
}
