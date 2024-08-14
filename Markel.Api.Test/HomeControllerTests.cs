using Markel.Api.Controllers;
using Markel.Service;
using Markel.Service.Exceptions;
using INT = Markel.Service.InternalModel;
using EXT = Markel.Api.ExternalModel;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Sdk;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Markel.Api.Test
{
    public class HomeControllerTests
    {
        //claim constants
        const int COMPANY_ID = 456;
        const string CLAIM_REF = "UCR";
        const string CLAIM_DATE = "2024-04-01";
        const string LOSS_DATE = "2024-03-12";
        const string ASSURED_NAME = "assured name";
        const decimal INCURRED_LOSS = 3.14m;
        const bool CLOSED = false;


        const int NEW_COMPANY_ID = 456;
        const string NEW_CLAIM_REF = "new claim ref";
        const string NEW_CLAIM_DATE = "2027-04-01";
        const string NEW_LOSS_DATE = "2028-03-12";
        const string NEW_ASSURED_NAME = "assuredly this is a name";
        const decimal NEW_INCURRED_LOSS = 6.28m;
        const bool NEW_CLOSED = true;

        private readonly Mock<ILogger<HomeController>> _logger;
        private readonly Mock<IMarkelService> _markelService;

        public HomeControllerTests()
        {
            _logger = new Mock<ILogger<HomeController>>();
            _markelService = new Mock<IMarkelService>();
        }

        #region "test methods"

        [Fact]
        public async void GetCompany_Exception()
        {
            const string ERR_MESSAGE = "get company error message";

            //ARRANGE
            _markelService.Setup(x => x.GetCompany(It.IsAny<int>()))
                .Throws(new Exception(ERR_MESSAGE));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetCompany(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.InternalServerError, ((ObjectResult)result.Result).StatusCode);


            _markelService.Verify(x => x.GetCompany(It.IsAny<int>()), Times.Once());

            _logger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(x => x == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == ERR_MESSAGE && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

        }

        [Fact]
        public async void GetCompany_ReturnsNothing()
        {
            //ARRANGE
            var emptyReturn = new INT.Company();
            _markelService.Setup(x => x.GetCompany(It.IsAny<int>()))
                .Returns(Task.FromResult(emptyReturn));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetCompany(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.NotFound, ((StatusCodeResult)result.Result).StatusCode);


            _markelService.Verify(x => x.GetCompany(It.IsAny<int>()), Times.Once());

        }

        [Fact]
        public async void GetCompany_ReturnsNull()
        {
            //ARRANGE
            INT.Company nullReturn = null;
            _markelService.Setup(x => x.GetCompany(It.IsAny<int>()))
                .Returns(Task.FromResult(nullReturn));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetCompany(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.NotFound, ((StatusCodeResult)result.Result).StatusCode);


            _markelService.Verify(x => x.GetCompany(It.IsAny<int>()), Times.Once());

        }

        [Fact]
        public async void GetCompany_HappyPath()
        {
            const bool ACTIVE = true;
            const string ADDRESS1 = "Address1";
            const string ADDRESS2 = "Address2";
            const string ADDRESS3 = "Address3";
            const string POSTCODE = "XX11 0PD";

            const string COUNTRY = "TPLAC";
            const int COMPANY_ID = 456;
            const string INSURANCE_END_DATE = "2024-04-01";
            const string COMPANY_NAME = "company name";
            //ARRANGE
            var company = new INT.Company
            {
                Id = COMPANY_ID,
                Active = true,
                Address1 = ADDRESS1,
                Address2 = ADDRESS2,
                Address3 = ADDRESS3,
                Postcode = POSTCODE,
                Country = COUNTRY,
                InsuranceEndDate = DateTime.Parse(INSURANCE_END_DATE),
                Name = COMPANY_NAME
            };

            _markelService.Setup(x => x.GetCompany(It.IsAny<int>()))
                .Returns(Task.FromResult(company));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetCompany(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<EXT.Company>(result.Value);
            var companyResult = (EXT.Company)result.Value;
            Assert.Equal(COMPANY_ID, company.Id);
            Assert.Equal(COMPANY_NAME, company.Name);
            Assert.Equal(ACTIVE, company.Active);
            Assert.Equal(ADDRESS1, company.Address1);
            Assert.Equal(ADDRESS2, company.Address2);
            Assert.Equal(ADDRESS3, company.Address3);
            Assert.Equal(POSTCODE, company.Postcode);
            Assert.Equal(DateTime.Parse(INSURANCE_END_DATE), company.InsuranceEndDate);

            _markelService.Verify(x => x.GetCompany(It.IsAny<int>()), Times.Once());

        }

        [Fact]
        public async void GetClaims_Exception()
        {
            const int COMPANY_ID = 456;
            const string ERR_MESSAGE = "get claims error message";

            //ARRANGE
            _markelService.Setup(x => x.GetClaims(It.IsAny<int>()))
                .Throws(new Exception(ERR_MESSAGE));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaims(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.InternalServerError, ((ObjectResult)result.Result).StatusCode);


            _markelService.Verify(x=>x.GetClaims(It.Is<int>(y=>y== COMPANY_ID)), Times.Once());

            _logger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(x => x == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == ERR_MESSAGE && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

        }

        [Fact]
        public async void GetClaims_ReturnsNothing()
        {
            const int COMPANY_ID = 456;

            //ARRANGE
            var emptyReturn = new List<INT.Claim>();
            _markelService.Setup(x => x.GetClaims(It.IsAny<int>()))
                .ReturnsAsync(emptyReturn);

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaims(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.NoContent, ((StatusCodeResult)result.Result).StatusCode);


            _markelService.Verify(x => x.GetClaims(It.Is<int>(y => y == COMPANY_ID)), Times.Once());

        }

        [Fact]
        public async void GetClaims_ReturnsNull()
        {
            const int COMPANY_ID = 456;

            //ARRANGE
            List<INT.Claim>? nullReturn = null;
            _markelService.Setup(x => x.GetClaims(It.IsAny<int>()))
                .ReturnsAsync(nullReturn);

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaims(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.NoContent, ((StatusCodeResult)result.Result).StatusCode);


            _markelService.Verify(x => x.GetClaims(It.Is<int>(y => y == COMPANY_ID)), Times.Once());

        }

        [Fact]
        public async void GetClaims_HappyPath()
        {
            const int COMPANY_ID = 456;
            const string UCR = "UNIVERSLA CLAIM REF";
            const string CLAIM_DATE = "2024-04-01";
            const string LOSS_DATE= "2024-03-12";
            const string ASSURED_NAME = "assured name";
            const decimal INCURRED_LOSS = 3.14m;
            const bool CLOSED = false;

            //ARRANGE
            var claims = new List<INT.Claim>();
            claims.Add(new INT.Claim
            {
                CompanyId = COMPANY_ID,
                Ucr = UCR,
                ClaimDate = DateTime.Parse(CLAIM_DATE),
                LossDate = DateTime.Parse(LOSS_DATE),
                AssuredName = ASSURED_NAME,
                IncurredLoss = INCURRED_LOSS,
                Closed = CLOSED,
            });

            _markelService.Setup(x => x.GetClaims(It.IsAny<int>()))
                .ReturnsAsync(claims);

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaims(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<List<EXT.Claim>>(result.Value);
            var claimsResult = (List<EXT.Claim>)result.Value;
            Assert.NotEmpty(claimsResult);
            Assert.Equal(COMPANY_ID, claimsResult.First().CompanyId);
            Assert.Equal(UCR, claimsResult.First().Ucr);
            Assert.Equal(DateTime.Parse(CLAIM_DATE), claimsResult.First().ClaimDate);
            Assert.Equal(DateTime.Parse(LOSS_DATE), claimsResult.First().LossDate);
            Assert.Equal(ASSURED_NAME, claimsResult.First().AssuredName);
            Assert.Equal(COMPANY_ID, claimsResult.First().CompanyId);
            Assert.Equal(INCURRED_LOSS, claimsResult.First().IncurredLoss);
            Assert.Equal(CLOSED, claimsResult.First().Closed);

            _markelService.Verify(x => x.GetClaims(It.Is<int>(y => y == COMPANY_ID)), Times.Once());

        }



        [Fact]
        public async void GetClaim_Exception()
        {
            const string CLAIM_REF = "UCR";
            const string ERR_MESSAGE = "get claim error message";

            //ARRANGE
            _markelService.Setup(x => x.GetClaim(It.IsAny<string>()))
                .Throws(new Exception(ERR_MESSAGE));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaimDetails(CLAIM_REF);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.InternalServerError, ((ObjectResult)result.Result).StatusCode);


            _markelService.Verify(x => x.GetClaim(It.Is<string>(y => y == CLAIM_REF)), Times.Once());

            _logger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(x => x == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == ERR_MESSAGE && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

        }

        [Fact]
        public async void GetClaim_ReturnsNothing()
        {
            const string CLAIM_REF = "UCR";

            //ARRANGE
            var emptyReturn = new INT.Claim();
            _markelService.Setup(x => x.GetClaim(It.IsAny<string>()))
                .Returns(Task.FromResult(emptyReturn));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaimDetails(CLAIM_REF);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.NotFound, ((StatusCodeResult)result.Result).StatusCode);


            _markelService.Verify(x => x.GetClaim(It.Is<string>(y => y == CLAIM_REF)), Times.Once());

        }

        [Fact]
        public async void GetClaim_ReturnsNull()
        {
            const string CLAIM_REF = "UCR";

            //ARRANGE
            INT.Claim? nullReturn = null;
            _markelService.Setup(x => x.GetClaim(It.IsAny<string>()))
                .Returns(Task.FromResult(nullReturn));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaimDetails(CLAIM_REF);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.IsType<StatusCodeResult>(result.Result);
            Assert.Equal((int)System.Net.HttpStatusCode.NotFound, ((StatusCodeResult)result.Result).StatusCode);


            _markelService.Verify(x => x.GetClaim(It.Is<string>(y => y == CLAIM_REF)), Times.Once());

        }

        [Fact]
        public async void GetClaim_HappyPath()
        {

            //ARRANGE
            var claim = GetClaim(CLAIM_REF, COMPANY_ID,CLAIM_DATE, LOSS_DATE,ASSURED_NAME, INCURRED_LOSS, CLOSED);

            _markelService.Setup(x => x.GetClaim(It.IsAny<string>()))
                .Returns(Task.FromResult(claim));

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaimDetails(CLAIM_REF);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.IsType<EXT.Claim>(result.Value);
            var claimResult = (EXT.Claim)result.Value;
            Assert.Equal(COMPANY_ID, claimResult.CompanyId);
            Assert.Equal(CLAIM_REF, claimResult.Ucr);
            Assert.Equal(DateTime.Parse(CLAIM_DATE), claimResult.ClaimDate);
            Assert.Equal(DateTime.Parse(LOSS_DATE), claimResult.LossDate);
            Assert.Equal(ASSURED_NAME, claimResult.AssuredName);
            Assert.Equal(COMPANY_ID, claimResult.CompanyId);
            Assert.Equal(INCURRED_LOSS, claimResult.IncurredLoss);
            Assert.Equal(CLOSED, claimResult.Closed);
            int ExpectedClaimAge = (DateTime.Now - DateTime.Parse(CLAIM_DATE)).Days;
            Assert.Equal(ExpectedClaimAge, claimResult.AgeInDays);

            _markelService.Verify(x => x.GetClaim(It.Is<string>(y => y == CLAIM_REF)), Times.Once());
        }

        [Fact]
        public async void UpdateClaim_NullParameterPassed()
        {
            //ARRANGE
            EXT.Claim? newClaimDetails = null;

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.UpdateClaim(newClaimDetails);

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)System.Net.HttpStatusCode.UnprocessableEntity, ((StatusCodeResult)result).StatusCode);

            _markelService.Verify(x => x.UpdateClaim(It.IsAny<INT.Claim>()), Times.Never);

        }


        [Fact]
        public async void UpdateClaim_Exception()
        {
            const string ERR_MESSAGE = "update claim error message";

            //ARRANGE
            var claim = GetClaim(CLAIM_REF, COMPANY_ID, CLAIM_DATE, LOSS_DATE, ASSURED_NAME, INCURRED_LOSS, CLOSED);

            _markelService.Setup(x => x.UpdateClaim(It.IsAny<INT.Claim>()))
                .Throws(new Exception(ERR_MESSAGE));

            //ACT
            var sut = GetSoftwareUnderTest();
            var newClaimDetails = GetExternalClaim(CLAIM_REF, NEW_COMPANY_ID, NEW_CLAIM_DATE, NEW_LOSS_DATE, NEW_ASSURED_NAME, NEW_INCURRED_LOSS, NEW_CLOSED);
            var result = await sut.UpdateClaim(newClaimDetails);

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)System.Net.HttpStatusCode.InternalServerError, ((ObjectResult)result).StatusCode);


            _markelService.Verify(x => x.UpdateClaim(It.IsAny<INT.Claim>()), Times.Once());

            _logger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(x => x == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == ERR_MESSAGE && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

        }

        [Fact]
        public async void UpdateClaim_ClaimNotFound()
        {

            //ARRANGE
            var claim = GetClaim(CLAIM_REF, COMPANY_ID, CLAIM_DATE, LOSS_DATE, ASSURED_NAME, INCURRED_LOSS, CLOSED);

            _markelService.Setup(x => x.UpdateClaim(It.IsAny<INT.Claim>()))
                .Throws(new ClaimNotFoundException());

            //ACT
            var sut = GetSoftwareUnderTest();
            var newClaimDetails = GetExternalClaim(NEW_CLAIM_REF, NEW_COMPANY_ID, NEW_CLAIM_DATE, NEW_LOSS_DATE, NEW_ASSURED_NAME, NEW_INCURRED_LOSS, NEW_CLOSED);
            var result = await sut.UpdateClaim(newClaimDetails);

            //ASSERT
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)System.Net.HttpStatusCode.NotFound, ((ObjectResult)result).StatusCode);


            _markelService.Verify(x => x.UpdateClaim(It.IsAny<INT.Claim>()), Times.Once());

        }



        [Fact]
        public async void UpdateClaim_HappyPath()
        {

            //ARRANGE
            var claim = GetClaim(CLAIM_REF, COMPANY_ID, CLAIM_DATE, LOSS_DATE, ASSURED_NAME, INCURRED_LOSS, CLOSED);

            _markelService.Setup(x => x.UpdateClaim(It.IsAny<INT.Claim>()));

            //ACT
            var sut = GetSoftwareUnderTest();
            var newClaimDetails = GetExternalClaim(CLAIM_REF, NEW_COMPANY_ID, NEW_CLAIM_DATE, NEW_LOSS_DATE, NEW_ASSURED_NAME, NEW_INCURRED_LOSS, NEW_CLOSED);
            var result = await sut.UpdateClaim(newClaimDetails);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);

            _markelService.Verify(x => x.UpdateClaim(It.Is<INT.Claim>(y=>y.Ucr == CLAIM_REF
                && y.CompanyId == NEW_COMPANY_ID
                && y.ClaimDate == DateTime.Parse(NEW_CLAIM_DATE)
                && y.LossDate == DateTime.Parse(NEW_LOSS_DATE)
                && y.IncurredLoss == NEW_INCURRED_LOSS
                && y.AssuredName == NEW_ASSURED_NAME
                && y.Closed == NEW_CLOSED)), Times.Once());

        }
        #endregion

        #region "helper methods"

        private HomeController GetSoftwareUnderTest()
        {
            return new HomeController(_logger.Object, _markelService.Object);
        }

        private INT.Claim GetClaim(string ucr, int companyId, string claimDate, string lossDate, string assuredName, decimal incurredLoss, bool closed) 
        { 
            return new INT.Claim
            {
                Ucr = ucr,
                CompanyId = companyId,
                ClaimDate = DateTime.Parse(claimDate),
                LossDate = DateTime.Parse(lossDate),
                AssuredName = assuredName,
                IncurredLoss = incurredLoss,
                Closed = closed,
            };
        }

        private EXT.Claim GetExternalClaim(string ucr, int companyId, string claimDate, string lossDate, string assuredName, decimal incurredLoss, bool closed)
        {
            return new EXT.Claim
            {
                Ucr = ucr,
                CompanyId = companyId,
                ClaimDate = DateTime.Parse(claimDate),
                LossDate = DateTime.Parse(lossDate),
                AssuredName = assuredName,
                IncurredLoss = incurredLoss,
                Closed = closed,
            };
        }


        #endregion
    }
}