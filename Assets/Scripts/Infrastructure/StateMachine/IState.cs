namespace Infrastructure.Bootstrap
{
    public interface IState
    {
        void Enter();
        void Exit();
    }
}