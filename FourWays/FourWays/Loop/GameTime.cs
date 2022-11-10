namespace FourWays.Loop
{
    public class GameTime
    {
        private float _deltaTime = 0f;
        private float _timeScale = 1f;

        internal float TimeScale { get => _timeScale; set => _timeScale = value; }
        internal float DeltaTime { get => _deltaTime * _timeScale; set => _deltaTime = value; }
        internal float DeltaTimeUnscaled { get => _deltaTime; }

        internal float TotalTimeElapsed { get; private set; }

        public GameTime()
        {

        }

        internal void Update(float deltaTime, float totalTimeElapsed)
        {
            _deltaTime = deltaTime;
            TotalTimeElapsed = totalTimeElapsed;
        }
    }
}