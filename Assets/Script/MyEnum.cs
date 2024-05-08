namespace Script
{
    public enum PlayerState
    {
        ChoosingAnswer,
        ChoosingTarget,
    }
    
    public enum GameState
    {
        WaitingStart,
        PLaying,
        End,
    }

    public enum ActionType
    {
        AttackNormal,
        UseSkill,
    }

    public enum ErrorCode
    {
        FullTeam,
        JoinLate,
    }
}