namespace Services
{
    public interface ITrackEvents
    {
        void OnLevelStarted(int level);
        void OnLevelCompleted(int level, int score = 0);
        void OnLevelFailed(int level, int score = 0);
    }
}
