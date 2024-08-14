using Xunit;
using Moq;
using Moq.EntityFrameworkCore;
using DL = Markel.DataLayer;
using INT = Markel.Service.InternalModel;
using Xunit.Sdk;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Markel.Service.InternalModel;
using Markel.Service.Exceptions;

namespace Markel.Service.Test
{
    public class MarkelServiceTests
    {
        const int COMPANY_ID = 456;
        const string CLAIM_REF = "8798";

        //claim constants
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

        private readonly Mock<DL.MarkelContext> _dbContextMock;
        private readonly Mock<DatabaseFacade> _dbFacade;

        private readonly Mock<IDbContextTransaction> _dbtrans;


        public MarkelServiceTests()
        {
            _dbContextMock = new Mock<DL.MarkelContext>();
            _dbFacade = new Mock<DatabaseFacade>(_dbContextMock.Object);
            _dbtrans = new Mock<IDbContextTransaction>();
            
        }

        #region "test methods"


        [Fact]
        public async void GetCompany_Exception()
        {
            const string ERR_MESSAGE = "get company error";

            //ARRANGE
            _dbContextMock.Setup(x => x.Companies).Throws(new Exception(ERR_MESSAGE));

            //ACT
            var sut = GetSoftwareUnderTest();
            Func<Task> testcode = async () => { await sut.GetCompany(COMPANY_ID); };
            var ex = await Record.ExceptionAsync(testcode);

            //ASSERT
            Assert.NotNull(ex);
            Assert.Equal(ERR_MESSAGE, ex.Message);

        }

        [Fact]
        public async void GetCompany_NotFound()
        {
            //ARRANGE

            var emptyCompanies = new List<DL.Company>();
            _dbContextMock.Setup(x => x.Companies).ReturnsDbSet(emptyCompanies);

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetCompany(COMPANY_ID);

            //ASSERT
            Assert.Null(result);

        }

        [Fact]
        public async void GetCompany_HappyPath_WithoutActiveClaim()
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

            var companies = new List<DL.Company> { 
                new DL.Company {
                    Id= COMPANY_ID, 
                    Active = ACTIVE, 
                    Address1 = ADDRESS1, 
                    Address2=ADDRESS2, 
                    Address3= ADDRESS3,
                    Country=COUNTRY,
                    InsuranceEndDate=DateTime.Parse(INSURANCE_END_DATE),
                    Name=COMPANY_NAME, 
                    Postcode = POSTCODE
                } 
            };
            _dbContextMock.Setup(x => x.Companies).ReturnsDbSet(companies);

            var claims = new List<DL.Claim>();
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(claims);

            //ACT
            var sut = GetSoftwareUnderTest();
            INT.Company result = await sut.GetCompany(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);

            Assert.Equal(COMPANY_ID, result.Id);
            Assert.Equal(COMPANY_NAME, result.Name);
            Assert.Equal(ACTIVE, result.Active);
            Assert.Equal(ADDRESS1, result.Address1);
            Assert.Equal(ADDRESS2, result.Address2);
            Assert.Equal(ADDRESS3, result.Address3);
            Assert.Equal(POSTCODE, result.Postcode);
            Assert.Equal(DateTime.Parse(INSURANCE_END_DATE), result.InsuranceEndDate);
            Assert.False(result.HasActiveClaim);
        }



        [Fact]
        public async void GetCompany_HappyPath_WithActiveClaim()
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

            var companies = new List<DL.Company> {
                new DL.Company {
                    Id= COMPANY_ID,
                    Active = ACTIVE,
                    Address1 = ADDRESS1,
                    Address2=ADDRESS2,
                    Address3= ADDRESS3,
                    Country=COUNTRY,
                    InsuranceEndDate=DateTime.Parse(INSURANCE_END_DATE),
                    Name=COMPANY_NAME,
                    Postcode = POSTCODE
                }
            };
            _dbContextMock.Setup(x => x.Companies).ReturnsDbSet(companies);

            var claims = new List<DL.Claim>{
             new DL.Claim {
                 CompanyId= COMPANY_ID,
                 Closed = false,
             }
            };
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(claims);

            //ACT
            var sut = GetSoftwareUnderTest();
            INT.Company result = await sut.GetCompany(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);

            Assert.Equal(COMPANY_ID, result.Id);
            Assert.Equal(COMPANY_NAME, result.Name);
            Assert.Equal(ACTIVE, result.Active);
            Assert.Equal(ADDRESS1, result.Address1);
            Assert.Equal(ADDRESS2, result.Address2);
            Assert.Equal(ADDRESS3, result.Address3);
            Assert.Equal(POSTCODE, result.Postcode);
            Assert.Equal(DateTime.Parse(INSURANCE_END_DATE), result.InsuranceEndDate);
            Assert.True(result.HasActiveClaim);
        }

        [Fact]
        public async void GetClaims_Exception()
        {
            const string ERR_MESSAGE = "get claims error";

            //ARRANGE
            _dbContextMock.Setup(x => x.Claims).Throws(new Exception(ERR_MESSAGE));

            //ACT
            var sut = GetSoftwareUnderTest();
            Func<Task> testcode = async () => { await sut.GetClaims(COMPANY_ID); };
            var ex = await Record.ExceptionAsync(testcode);

            //ASSERT
            Assert.NotNull(ex);
            Assert.Equal(ERR_MESSAGE, ex.Message);

        }

        [Fact]
        public async void GetClaims_NotFound()
        {
            //ARRANGE

            var emptyClaims = new List<DL.Claim>();
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(emptyClaims);

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaims(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);

        }

        [Fact]
        public async void GetClaims_HappyPath()
        {
            const string UCR = "UNIVERSaL CLAIM REF";
            const string CLAIM_DATE = "2024-04-01";
            const string LOSS_DATE = "2024-03-12";
            const string ASSURED_NAME = "assured name";
            const decimal INCURRED_LOSS = 3.14m;
            const bool CLOSED = false;

            //ARRANGE

            var claims = new List<DL.Claim>
            {
                 new DL.Claim 
                 {
                 CompanyId= COMPANY_ID,
                 AssuredName=ASSURED_NAME,
                 ClaimDate = DateTime.Parse(CLAIM_DATE),
                 LossDate = DateTime.Parse(LOSS_DATE),
                 IncurredLoss=INCURRED_LOSS,
                 Ucr = UCR,
                 Closed = CLOSED,
                 }
             };
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(claims);

            //ACT
            var sut = GetSoftwareUnderTest();
            IEnumerable<INT.Claim> result = await sut.GetClaims(COMPANY_ID);

            //ASSERT
            Assert.NotNull(result);
            Assert.NotEmpty(result);


            Assert.Equal(COMPANY_ID, result.First().CompanyId);
            Assert.Equal(UCR, result.First().Ucr);
            Assert.Equal(ASSURED_NAME, result.First().AssuredName);
            Assert.Equal(DateTime.Parse(CLAIM_DATE), result.First().ClaimDate);
            Assert.Equal(DateTime.Parse(LOSS_DATE), result.First().LossDate);
            Assert.Equal(INCURRED_LOSS, result.First().IncurredLoss);
            Assert.Equal(CLOSED, result.First().Closed);
        }

        [Fact]
        public async void GetClaim_Exception()
        {
            const string ERR_MESSAGE = "get claim error";

            //ARRANGE
            _dbContextMock.Setup(x => x.Claims).Throws(new Exception(ERR_MESSAGE));

            //ACT
            var sut = GetSoftwareUnderTest();
            Func<Task> testcode = async () => { await sut.GetClaim(CLAIM_REF); };
            var ex = await Record.ExceptionAsync(testcode);

            //ASSERT
            Assert.NotNull(ex);
            Assert.Equal(ERR_MESSAGE, ex.Message);
        }

        [Fact]
        public async void GetClaim_NotFound()
        {
            //ARRANGE

            var emptyClaims = new List<DL.Claim>();
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(emptyClaims);

            //ACT
            var sut = GetSoftwareUnderTest();
            var result = await sut.GetClaim(CLAIM_REF);

            //ASSERT
            Assert.Null(result);

        }

        [Fact]
        public async void GetClaim_HappyPath()
        {
            const string CLAIM_DATE = "2024-04-01";
            const string LOSS_DATE = "2024-03-12";
            const string ASSURED_NAME = "assured name";
            const decimal INCURRED_LOSS = 3.14m;
            const bool CLOSED = false;

            //ARRANGE

            var claims = new List<DL.Claim>
            {
                 new DL.Claim
                 {
                 CompanyId= COMPANY_ID,
                 AssuredName=ASSURED_NAME,
                 ClaimDate = DateTime.Parse(CLAIM_DATE),
                 LossDate = DateTime.Parse(LOSS_DATE),
                 IncurredLoss=INCURRED_LOSS,
                 Ucr = CLAIM_REF,
                 Closed = CLOSED,
                 }
             };
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(claims);

            //ACT
            var sut = GetSoftwareUnderTest();
            INT.Claim result = await sut.GetClaim(CLAIM_REF);

            //ASSERT
            Assert.NotNull(result);


            Assert.Equal(COMPANY_ID, result.CompanyId);
            Assert.Equal(CLAIM_REF, result.Ucr);
            Assert.Equal(ASSURED_NAME, result.AssuredName);
            Assert.Equal(DateTime.Parse(CLAIM_DATE), result.ClaimDate);
            Assert.Equal(DateTime.Parse(LOSS_DATE), result.LossDate);
            Assert.Equal(INCURRED_LOSS, result.IncurredLoss);
            Assert.Equal(CLOSED, result.Closed);
        }

        [Fact]
        public async void UpdateClaim_Exception()
        {
            const string ERR_MESSAGE = "get claim error";

            //ARRANGE
            var claims = new List<DL.Claim>
            {
                 GetExistingClaim(CLAIM_REF, COMPANY_ID, CLAIM_DATE,LOSS_DATE, ASSURED_NAME, INCURRED_LOSS, CLOSED)
            };
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(claims);
            _dbContextMock.Setup(x => x.SaveChangesAsync(new CancellationToken())).Throws(new Exception(ERR_MESSAGE));
            _dbFacade.Setup(x => x.BeginTransaction()).Returns(_dbtrans.Object);
            _dbContextMock.Setup(x=>x.Database).Returns(_dbFacade.Object);


            INT.Claim newClaimDetails = GetNewClaimDetails(CLAIM_REF, NEW_COMPANY_ID, NEW_CLAIM_DATE, NEW_LOSS_DATE, NEW_ASSURED_NAME, NEW_INCURRED_LOSS, NEW_CLOSED);

            //ACT
            var sut = GetSoftwareUnderTest();
            Func<Task> testcode = async () =>  { await sut.UpdateClaim(newClaimDetails); };
            var ex = await Record.ExceptionAsync(testcode);

            //ASSERT
            Assert.NotNull(ex);
            Assert.Equal(ERR_MESSAGE, ex.Message);
        }



        [Fact]
        public async void UpdateClaim_RowsAffectedFailure()
        {
            //ARRANGE
            var claims = new List<DL.Claim>
            {
                 GetExistingClaim(CLAIM_REF, COMPANY_ID, CLAIM_DATE,LOSS_DATE, ASSURED_NAME, INCURRED_LOSS, CLOSED)
            };
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(claims);
            _dbContextMock.Setup(x => x.SaveChangesAsync(new CancellationToken())).Returns(Task.FromResult(2));
            _dbFacade.Setup(x => x.BeginTransaction()).Returns(_dbtrans.Object);
            _dbContextMock.Setup(x => x.Database).Returns(_dbFacade.Object);


            INT.Claim newClaimDetails = GetNewClaimDetails(CLAIM_REF, NEW_COMPANY_ID, NEW_CLAIM_DATE, NEW_LOSS_DATE, NEW_ASSURED_NAME, NEW_INCURRED_LOSS, NEW_CLOSED);

            //ACT
            var sut = GetSoftwareUnderTest();
            Func<Task> testcode = async () => { await sut.UpdateClaim(newClaimDetails); };
            var ex = await Record.ExceptionAsync(testcode);

            //ASSERT
            Assert.NotNull(ex);
            Assert.Equal(MarkelService.ERR_FAILED_TO_SAVE_CLAIM, ex.Message);

        }

        [Fact]
        public async void UpdateClaim_NotFound()
        {
            //ARRANGE

            var emptyClaims = new List<DL.Claim>();
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(emptyClaims);

            INT.Claim newClaimDetails = GetNewClaimDetails(CLAIM_REF, NEW_COMPANY_ID, NEW_CLAIM_DATE, NEW_LOSS_DATE, NEW_ASSURED_NAME, NEW_INCURRED_LOSS, NEW_CLOSED);

            //ACT
            var sut = GetSoftwareUnderTest();
            Func<Task> testcode = async () => { await sut.UpdateClaim(newClaimDetails); };
            var ex = await Record.ExceptionAsync(testcode);

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<ClaimNotFoundException>(ex);

        }

        [Fact]
        public void UpdateClaim_HappyPath()
        {

            //ARRANGE
            var claims = new List<DL.Claim>
            {
                 GetExistingClaim(CLAIM_REF, COMPANY_ID, CLAIM_DATE,LOSS_DATE, ASSURED_NAME, INCURRED_LOSS, CLOSED) 
            };
            _dbContextMock.Setup(x => x.Claims).ReturnsDbSet(claims);

            INT.Claim newClaimDetails = GetNewClaimDetails(CLAIM_REF, NEW_COMPANY_ID, NEW_CLAIM_DATE, NEW_LOSS_DATE, NEW_ASSURED_NAME, NEW_INCURRED_LOSS, NEW_CLOSED);


            //ACT
            var sut = GetSoftwareUnderTest();
            sut.UpdateClaim(newClaimDetails);

            //ASSERT
        }

        #endregion



        #region "helper methods"

        private MarkelService GetSoftwareUnderTest()
        {
            return new MarkelService(_dbContextMock.Object);
        }

        private DL.Claim GetExistingClaim(string ucr, int companyId, string claimDate, string lossDate, string assuredName, decimal incurredLoss, bool closed)
        {
            return new DL.Claim
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

        private INT.Claim GetNewClaimDetails(string ucr, int companyId, string claimDate, string lossDate, string assuredName, decimal incurredLoss, bool closed)
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
        #endregion
    }
}