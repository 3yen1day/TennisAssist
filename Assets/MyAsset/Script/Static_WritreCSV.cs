using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class Static_WriteCSV
{
    //ファイルの保存場所
    public static StreamWriter TargetFilePath = File.AppendText(Application.dataPath + @"\TextFile\25\Target.txt"); //Application.dataPath = AssetFolder
    public static StreamWriter RacketSpeedFilePath = File.AppendText(Application.dataPath + @"\TextFile\25\RacketSpeed.txt");
    public static StreamWriter RacketQuatFilePath = File.AppendText(Application.dataPath + @"\TextFile\25\RacketQuat.txt");
    public static StreamWriter RacketRotFilePath = File.AppendText(Application.dataPath + @"\TextFile\25\RacketRot.txt");
    public static StreamWriter FallPointFilePath = File.AppendText(Application.dataPath + @"\TextFile\25\FallPoint.txt");

    //ターゲットにHITしたとき
    public static void WriteHitTargetNum()
    {
        if(!(Static_Paramater.IsPractice)) TargetFilePath.WriteLine("T");
    }

    //ターゲットを外したとき
    public static void WriteMissTargetNum()
    {
        if (!(Static_Paramater.IsPractice)) TargetFilePath.WriteLine("F");
    }

    //ラケットスピード
    public static void WriteRacketSpeed(float speed)
    {
        if (!(Static_Paramater.IsPractice)) RacketSpeedFilePath.WriteLine(speed.ToString());
    }

    //ラケットの角度（クォータニオン）
    public static void WriteRacketQuat(Quaternion rot)
    {
        if (!(Static_Paramater.IsPractice))
        {
            RacketQuatFilePath.WriteLine(rot.ToString());
            WriteRacketRot(rot.eulerAngles);
        }
    }

    //ラケットの角度（Vector3）
    private static void WriteRacketRot(Vector3 rot)
    {
        if (!(Static_Paramater.IsPractice)) RacketRotFilePath.WriteLine(rot.ToString() );
    }

    //落下地点座標
    public static void WriteFallPoint(float dist)
    {
        if (!(Static_Paramater.IsPractice)) FallPointFilePath.WriteLine(Static_Paramater.GetRemainingBallNum() + ", " + dist.ToString());
    }
    
    //Tabで改行
    //ゴースト軌道で取ってる情報を複数個で一つにまとめる
    public static void NextRow()
    {
        RacketSpeedFilePath.WriteLine("    ");
        RacketQuatFilePath.WriteLine("    ");
        RacketRotFilePath.WriteLine("    ");
    }

    public static void EndTextFile()
    {
        TargetFilePath.WriteLine("/n");
        RacketSpeedFilePath.WriteLine("/n");
        RacketQuatFilePath.WriteLine("/n");
        RacketRotFilePath.WriteLine("/n");
        FallPointFilePath.WriteLine("/n");
    }

    public static void CloseTextfile()
    {
        TargetFilePath.Flush();
        TargetFilePath.Close();

        RacketSpeedFilePath.Flush();
        RacketSpeedFilePath.Close();

        RacketQuatFilePath.Flush();
        RacketQuatFilePath.Close();

        RacketRotFilePath.Flush();
        RacketRotFilePath.Close();

        FallPointFilePath.Flush();
        FallPointFilePath.Close();
    }

}
