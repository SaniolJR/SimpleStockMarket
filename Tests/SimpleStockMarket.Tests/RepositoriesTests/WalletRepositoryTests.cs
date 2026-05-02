using Xunit;
using Moq;
using Services;
using Entities;
using Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.RepositoriesTests;

[Collection("postgres")]
public class WalletRepositoryIntegrationTests
{

    private readonly PostgresContainerFixture _fx;
    public WalletRepositoryIntegrationTests(PostgresContainerFixture fx) => _fx = fx;


    /*
    ============================
        GetStockByNameAsync
    ============================
    */


}
