namespace Enklu.IoTNet
{
    /// <summary>
    /// Configuration for Mamba.
    /// </summary>
    public class IoTNetConfiguration
    {
        public string MyceliumIp;
        public int MyceliumPort;
        public string TrellisUrl;
        public string Token;
        public string ExperienceId;

        /// <summary>
        /// Overrides settings.
        /// </summary>
        /// <param name="config">Configuration values to override with.</param>
        public void Override(IoTNetConfiguration config)
        {
            if (!string.IsNullOrEmpty(config.MyceliumIp))
            {
                MyceliumIp = config.MyceliumIp;
            }

            if (config.MyceliumPort > 0)
            {
                MyceliumPort = config.MyceliumPort;
            }

            if (!string.IsNullOrEmpty(config.TrellisUrl))
            {
                TrellisUrl = config.TrellisUrl;
            }

            if (!string.IsNullOrEmpty(config.Token))
            {
                Token = config.Token;
            }

            if (!string.IsNullOrEmpty(config.ExperienceId))
            {
                ExperienceId = config.ExperienceId;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"MyceliumIp: {MyceliumIp}, MyceliumPort: {MyceliumPort}, TrellisUrl: {TrellisUrl}, Token: {Token}, ExperienceId: {ExperienceId}";
        }
    }
}