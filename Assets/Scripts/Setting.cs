public struct Setting
{
    public int humanNum;
    public int computerNum;
    public int teamNum;

    public readonly int PlayerNum => humanNum + computerNum;

    public Setting(int humanNum, int computerNum, int teamNum)
    {
        this.humanNum = humanNum;
        this.computerNum = computerNum;
        this.teamNum = teamNum;
    }
}
