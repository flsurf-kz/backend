﻿using KarmaMarketplace.Application.Common.Interactors;

namespace LMS.Application.Study.UseCases.Courses
{
    public class ReadCourse : BaseUseCase<InputDTO, OutputDTO>
    {
        public ReadCourse() { }

        public async Task<OutputDTO> Execute(InputDTO dto)
        {
            return;
        }
    }
}
