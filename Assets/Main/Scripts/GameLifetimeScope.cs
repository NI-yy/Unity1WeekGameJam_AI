using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<EnemyManager>();
        builder.RegisterComponentInHierarchy<GameManager>();
        builder.RegisterComponentInHierarchy<GameUIManager>();
        builder.RegisterComponentInHierarchy<PlayerManager>();
    }
}
