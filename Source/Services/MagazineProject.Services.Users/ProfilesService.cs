﻿namespace MagazineProject.Services.Users
{
    using System.Linq;
    using System.Web.Helpers;

    using MagazineProject.Data.Common.Model;
    using MagazineProject.Data.Models;
    using MagazineProject.Data.UnitOfWork;
    using MagazineProject.Services.Common;
    using MagazineProject.Services.Common.User;
    using MagazineProject.Web.Infrastructure.Sanitizer;
    using MagazineProject.Web.Models.Area.Users.InputViewModels.Settings;

    public class ProfilesService : BaseService, IProfilesService
    {
        private readonly ISanitizer sanitizer;

        public ProfilesService(IUnitOfWorkData data, ISanitizer sanitizer)
            : base(data)
        {
            this.sanitizer = sanitizer;
        }

        public IQueryable<User> GetProfileById(string userId)
        {
            return this.Data
                .Users
                .All()
                .Where(u => u.Id == userId);
        }

        public void UpdateProfile(User model, UserProfileSettingsViewModel viewModel)
        {
            if (viewModel.InfoContent == null)
            {
                model.InfoContent = viewModel.InfoContent;
            }
            else
            {
                model.InfoContent = this.sanitizer.Sanitize(viewModel.InfoContent);
            }
            model.Email = viewModel.Email;
            model.FirstName = viewModel.FirstName;
            model.LastName = viewModel.LastName;
            this.UpdateUserImage(model);

            this.Data.SaveChanges();
        }

        public IQueryable<Comment> GetProfileComments(string userId)
        {
            var userComments = this.Data
                .Comments
                .All()
                .Where(c => c.AuthorId == userId &&
                            c.Status == Status.Published);

            return userComments;
        }

        public IQueryable<Post> GetProfilePosts(string userId)
        {
            var userPosts = this.Data
                .Posts
                .All()
                .Where(p => p.AuthorId == userId &&
                            p.Status == Status.Published);

            return userPosts;
        }

        private void UpdateUserImage(User model)
        {
            var photo = WebImage.GetImageFromRequest();
            if (photo != null)
            {
                photo.Resize(width: 300, height: 200, preserveAspectRatio: false, preventEnlarge: false);
                byte[] data = photo.GetBytes();

                model.UserImage.Content = data;
                model.UserImage.FileExtension = photo.ImageFormat;
            }
        }
    }
}
