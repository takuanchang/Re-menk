public struct Setting
{
    public int humanNum;
    public int computerNum;
    public int teamNum;

    public readonly int PlayersNum => humanNum + computerNum;

    /// <summary>
    /// 一人用(vs CPU)
    /// </summary>
    public readonly static Setting Single = new Setting(1, 1, 2);
    /// <summary>
    /// 二人用(vs Player)
    /// </summary>
    public readonly static Setting Double = new Setting(2, 0, 2);

    public Setting(int humanNum, int computerNum, int teamNum)
    {
        this.humanNum = humanNum;
        this.computerNum = computerNum;
        this.teamNum = teamNum;
    }
}
