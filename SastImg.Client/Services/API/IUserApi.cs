// <auto-generated>
//     This code was generated by Refitter.
// </auto-generated>


using Refit;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using SastImg.Client.Service.API;

#nullable enable annotations

namespace SastImg.Client.Service.API
{
    /// <summary>UpdateBiography</summary>
    [System.CodeDom.Compiler.GeneratedCode("Refitter", "1.5.0.0")]
    [Headers("Authorization: Bearer")]
    public partial interface IUserApi
    {
        /// <summary>UpdateBiography</summary>
        /// <remarks>Update the biography</remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the <see cref="IApiResponse"/> instance containing the result:
        /// <list type="table">
        /// <listheader>
        /// <term>Status</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>200</term>
        /// <description>OK</description>
        /// </item>
        /// </list>
        /// </returns>
        [Post("/api/users/biography")]
        Task<IApiResponse> UpdateBiographyAsync([Body] UpdateBiographyRequest body, CancellationToken cancellationToken = default);

        /// <summary>UpdateAvatar</summary>
        /// <remarks>Update the user avatar image.</remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the <see cref="IApiResponse"/> instance containing the result:
        /// <list type="table">
        /// <listheader>
        /// <term>Status</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>200</term>
        /// <description>OK</description>
        /// </item>
        /// </list>
        /// </returns>
        [Multipart]
        [Post("/api/users/avatar")]
        Task<IApiResponse> UpdateAvatarAsync(StreamPart avatar, CancellationToken cancellationToken = default);

        /// <summary>UpdateHeader</summary>
        /// <remarks>Update the user header image.</remarks>
        /// <returns>
        /// A <see cref="Task"/> representing the <see cref="IApiResponse"/> instance containing the result:
        /// <list type="table">
        /// <listheader>
        /// <term>Status</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>200</term>
        /// <description>OK</description>
        /// </item>
        /// </list>
        /// </returns>
        [Multipart]
        [Post("/api/users/header")]
        Task<IApiResponse> UpdateHeaderAsync(StreamPart header, CancellationToken cancellationToken = default);

        /// <summary>GetAvatar</summary>
        /// <remarks>Get the avatar image file of the specific user.</remarks>
        /// <param name="id">The user id.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the <see cref="IApiResponse"/> instance containing the result:
        /// <list type="table">
        /// <listheader>
        /// <term>Status</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>200</term>
        /// <description>OK</description>
        /// </item>
        /// </list>
        /// </returns>
        [Headers("Accept: text/plain, application/json, text/json")]
        [Get("/api/users/{id}/avatar")]
        Task<IApiResponse<Stream>> GetAvatarAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>GetHeader</summary>
        /// <remarks>Get the header image file of the specific user.</remarks>
        /// <param name="id">The user id.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the <see cref="IApiResponse"/> instance containing the result:
        /// <list type="table">
        /// <listheader>
        /// <term>Status</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>200</term>
        /// <description>OK</description>
        /// </item>
        /// </list>
        /// </returns>
        [Headers("Accept: text/plain, application/json, text/json")]
        [Get("/api/users/{id}/header")]
        Task<IApiResponse<FileStreamResult>> GetHeaderAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>GetProfile</summary>
        /// <remarks>Get the profile info of the specific user.</remarks>
        /// <param name="id">The user id.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the <see cref="IApiResponse"/> instance containing the result:
        /// <list type="table">
        /// <listheader>
        /// <term>Status</term>
        /// <description>Description</description>
        /// </listheader>
        /// <item>
        /// <term>200</term>
        /// <description>OK</description>
        /// </item>
        /// </list>
        /// </returns>
        [Headers("Accept: text/plain, application/json, text/json")]
        [Get("/api/users/{id}/profile")]
        Task<IApiResponse<UserProfileDto>> GetProfileInfoAsync(long id, CancellationToken cancellationToken = default);
    }

}