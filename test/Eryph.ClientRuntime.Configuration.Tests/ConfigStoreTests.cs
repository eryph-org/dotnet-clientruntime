using System;
using System.IO;
using System.Linq;
using System.Text;
using Eryph.IdentityModel.Clients;
using Moq;
using Xunit;

namespace Eryph.ClientRuntime.Configuration.Tests
{
    public class ConfigStoreTests
    {

        [Fact]
        public void GetClients_reads_only_valid_clients()
        {
            var filesystemMock = new Mock<IFileSystem>();
            var environmentMock = new Mock<IEnvironment>();
            environmentMock.Setup(x => x.FileSystem).Returns(filesystemMock.Object);


            // ReSharper disable once ConvertToUsingDeclaration
            using (var defaultConfigStream =
                  "{ \"clients\" : [ {\"id\" : \"id-1\" }, {\"id\" : \"id-2\" }, {\"id\" : \"id-3\" }  ]}".ToStream())
            {
                filesystemMock.Setup(x => x.FileExists(It.Is<string>(x2 => x2.EndsWith("default.config"))))
                    .Returns(true);
                filesystemMock.Setup(x => x.OpenStream(It.Is<string>(x2 => x2.EndsWith("default.config"))))
                    .Returns(defaultConfigStream);


                filesystemMock.Setup(x => x.FileExists(It.Is<string>(x2 =>
                    x2.EndsWith("id-1.key") || x2.EndsWith("id-3.key")))).Returns(true);

                using (var keyFileStream = TestData.PrivateKeyFileString.ToStream())
                {
                    filesystemMock.Setup(x => x.OpenStream(It.Is<string>(
                        x2 => x2.EndsWith(".key")
                    ))).Returns(keyFileStream);


                    var configStore =
                        ConfigStore.GetStore(ConfigStoreLocation.CurrentDirectory, environmentMock.Object);
                    var clientIds = configStore.GetClients().Select(x => x.Id).ToArray();

                    Assert.Contains("id-1", clientIds);
                    Assert.Contains("id-3", clientIds);
                }
            }
        }
        
        [Fact]
        public void Creates_new_ConfigStore_with_Endpoint()
        {
            var filesystemMock = new Mock<IFileSystem>();
            var environmentMock = new Mock<IEnvironment>();
            environmentMock.Setup(x => x.FileSystem).Returns(filesystemMock.Object);

            // ReSharper disable once ConvertToUsingDeclaration
            using (var memoryStream = new MemoryStream())
            {
                filesystemMock.Setup(x => x.CreateStream(It.Is<string>(
                    p => p.EndsWith("local.config")))).Returns(new WrappedStream(memoryStream));

                var writer = new ConfigStoresWriter(environmentMock.Object, "local");
                writer.AddEndpoint("endpointName", new Uri("http://localhost"));

                environmentMock.Verify();
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var content = streamReader.ReadToEnd();
                    Assert.Equal(
                        "{\r\n  \"endpoints\": {\r\n    \"endpointName\": \"http://localhost\"\r\n  }\r\n}",
                        content);
                }
            }
        }

    }
}
