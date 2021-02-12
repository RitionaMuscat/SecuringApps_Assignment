using AutoMapper;
using AutoMapper.QueryableExtensions;
using SecuringApps.Application.Interfaces;
using SecuringApps.Application.ViewModels;
using SecuringApps.Domain.Interfaces;
using SecuringApps.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecuringApps.Application.Services
{
    public class CommentsService : ICommentsService
    {
        private ICommentsRepository _commentsRepo;
        private IMapper _mapper;

        public CommentsService(ICommentsRepository commentsRepo, IMapper mapper)
        {
            _commentsRepo = commentsRepo;
            _mapper = mapper;
        }
        public void AddComments(CommentsViewModel data)
        {
            Comments comments = new Comments();
            comments.WorkId = data.StudentWork.Id;
            comments.comment = data.comment;
            comments.writtenBy = data.writtenBy;
            comments.writtenOn = data.writtenOn;
            _commentsRepo.AddComments(comments);

        }

        public void DeleteComments(Guid id)
        {
            if (_commentsRepo.GetComments(id) != null)
                _commentsRepo.DeleteComments(id);
        }

        public CommentsViewModel GetComments(Guid id)
        {
            Comments comments = _commentsRepo.GetComments(id);
            var resultingCommentsViewModel = _mapper.Map<CommentsViewModel>(comments);
            return resultingCommentsViewModel;
        }

        public System.Linq.IQueryable<CommentsViewModel> GetComments()
        {
            return _commentsRepo.GetComments().ProjectTo<CommentsViewModel>(_mapper.ConfigurationProvider);

        }
    }
}
