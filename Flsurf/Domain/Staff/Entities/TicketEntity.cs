﻿using Flsurf.Domain.Common;
using Flsurf.Domain.Files.Entities;
using Flsurf.Domain.Staff.Enums;
using Flsurf.Domain.Staff.Events;
using Flsurf.Domain.Staff.Exceptions;
using Flsurf.Domain.User.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flsurf.Domain.Staff.Entities
{
    public class TicketEntity : BaseAuditableEntity
    {
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        public string Subject { get; set; } = null!;
        public List<FileEntity> Files { get; set; } = [];
        public Guid? AnsweredCommentId { get; set; }
        [Required]
        public TicketStatus Status { get; private set; } = TicketStatus.Open;
        [ForeignKey("AssignedUserId")]
        public UserEntity? AssignedUser { get; private set; }
        public Guid? AssignedUserId { get; private set; }
        public ICollection<TicketCommentEntity> Comments { get; private set; } = [];
        [ForeignKey(nameof(UserEntity))]
        public Guid? ClosedById { get; private set; }
        public UserEntity CreatedBy { get; private set; } = null!;

        private void RaiseIfClosed()
        {
            if (Status == TicketStatus.Closed)
            {
                throw new TicketIsClosed(Id);
            }
        }

        public static TicketEntity Create(
            string text, string subject, UserEntity createdBy, List<FileEntity> files)
        {
            var ticket = new TicketEntity()
            {
                CreatedBy = createdBy,
                Text = text,
                Subject = subject,
                Files = files
            };

            ticket.AddDomainEvent(new TicketCreated(ticket));

            return ticket;
        }

        public void Accept(UserEntity byUser)
        {
            RaiseIfClosed();
            if (byUser.Id == AssignedUser?.Id)
            {
                // do nothing 
                return;
            }
            AssignedUser = byUser;

            SetStatus(TicketStatus.InProgress);

            AddDomainEvent(new TicketAccepted(byUser, this));
        }

        public void Close(UserEntity byUser)
        {
            RaiseIfClosed();

            ClosedById = byUser.Id;
            SetStatus(TicketStatus.Closed);
        }

        private void SetStatus(TicketStatus status)
        {
            Status = status;
            AddDomainEvent(new TicketStatusUpdated(this, status));
        }

        public TicketCommentEntity AddComment(UserEntity byUser, string text, ICollection<FileEntity> files, Guid? commentAnswerId = null)
        {
            RaiseIfClosed();

            TicketCommentEntity comment;
            if (commentAnswerId == null)
            {
                comment = TicketCommentEntity.Create(byUser, text, files);
            }
            else
            {
                comment = TicketCommentEntity.Create(byUser, text, files, (Guid)commentAnswerId);
            }
            Comments.Add(comment);

            AddDomainEvent(new TicketCommentAdded(comment, this));
            return comment;
        }

        public void RemoveComment(Guid commentId)
        {
            RaiseIfClosed();
            var comment = Comments.Where(x => x.Id == commentId).FirstOrDefault();
            if (comment != null)
            {
                Comments.Remove(comment);

                AddDomainEvent(new TicketCommentRemoved(comment, this));
            }
        }
    }
}
