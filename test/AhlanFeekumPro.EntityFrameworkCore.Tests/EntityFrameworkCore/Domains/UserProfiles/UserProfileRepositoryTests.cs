using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using AhlanFeekumPro.UserProfiles;
using AhlanFeekumPro.EntityFrameworkCore;
using Xunit;

namespace AhlanFeekumPro.EntityFrameworkCore.Domains.UserProfiles
{
    public class UserProfileRepositoryTests : AhlanFeekumProEntityFrameworkCoreTestBase
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileRepositoryTests()
        {
            _userProfileRepository = GetRequiredService<IUserProfileRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _userProfileRepository.GetListAsync(
                    name: "237417d0ce6342889ff8f242566523626dfa185d694e40b99e45dad59ad84923e24889ec94224f7e82ea635ee5f7751",
                    email: "034c2494cc714d30a06fef56f@bea3964d18c648f3adf44251f.com",
                    phoneNumber: "d714fd46f9d44c8581390a984fa1c798e955c4dbe6be4d9fbd6abce7b445580f28da637fb99f443087d9ff966cf85",
                    latitude: "94dd55aeffce4adead12cd4a12edbc1565f4438104eb4661afed7b26e64f",
                    longitude: "6693d01b6c5a47ae936ffd9d49330094b1dacd",
                    address: "77265f3b95bb45e8b58bb91",
                    profilePhoto: "8832697a2b74445f82f1b58a1c12123715049d90699a456394f5c0adb",
                    isSuperHost: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("96d4e38a-fe57-47a6-a235-25ceb1d18f0f"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _userProfileRepository.GetCountAsync(
                    name: "5f0257c40b8b49a68208e3af0cadfd3601886c",
                    email: "cae8ecdb4fa54dd4924440420aa705e01e771a@c958ad01ee3a43b6b00d658d61ff7bbb8c4c47.com",
                    phoneNumber: "e23507211d5843499c99f4cdd3541f89e73b0504a959",
                    latitude: "6bc724849e144086b22abfb1040961982a74",
                    longitude: "003c9620984a41d49eab0118856ee64f07d5717ef73b4f8ea8134e7d60038d5603e53082b",
                    address: "80d3cdd30b7c46478f2072f122b0c35c017266bb69e048719df7304d421193ebd56f3ba93c034a5a",
                    profilePhoto: "400271e921df4ef5abc5455",
                    isSuperHost: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}