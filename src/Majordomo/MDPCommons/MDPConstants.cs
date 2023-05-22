namespace MDPCommons;

public static class MDPConstants
{
    public const int HEARTBEAT_LIVENESS = 4; // 3-5 is reasonable
    public const int HEARTBEAT_INTERVAL_IN_MILIS = 2500;

    public const string MDP_CLIENT_HEADER = "MDPC01";
    public const string MDP_WORKER_HEADER = "MDPW01";
}
