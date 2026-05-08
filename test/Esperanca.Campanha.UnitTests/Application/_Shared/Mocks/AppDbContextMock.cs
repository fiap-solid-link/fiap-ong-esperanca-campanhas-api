using Esperanca.Campanha.Application._Shared;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using CampanhaAgg = Esperanca.Campanha.Domain.Campanhas.Campanha;

namespace Esperanca.Campanha.UnitTests.Application._Shared.Mocks;

public class AppDbContextMock
{
    public IAppDbContext Instance { get; }
    public DbSet<CampanhaAgg> CampanhasDbSet { get; private set; } = default!;

    public AppDbContextMock()
    {
        Instance = Substitute.For<IAppDbContext>();
        SetupCampanhas([]);
    }

    public AppDbContextMock SetupCampanhas(List<CampanhaAgg> data)
    {
        CampanhasDbSet = data.AsQueryable().BuildMockDbSet();
        Instance.Set<CampanhaAgg>().Returns(CampanhasDbSet);
        return this;
    }

    public AppDbContextMock SetupSaveChangesSuccess(int affected = 1)
    {
        Instance.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(affected);
        return this;
    }

    public AppDbContextMock SetupSaveChangesThrows(Exception exception)
    {
        Instance.SaveChangesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(exception);
        return this;
    }

    public void VerifyCampanhaAdded() =>
        CampanhasDbSet.Received(1).Add(Arg.Any<CampanhaAgg>());

    public void VerifyCampanhaNotAdded() =>
        CampanhasDbSet.DidNotReceive().Add(Arg.Any<CampanhaAgg>());

    public void VerifySaveChangesCalled() =>
        Instance.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

    public void VerifySaveChangesNotCalled() =>
        Instance.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
}
