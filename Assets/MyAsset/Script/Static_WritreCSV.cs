using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class Static_WriteCSV
{
    //ファイルの保存場所
    public static StreamWriter TargetFilePath = File.AppendText(Application.dataPath + @"\TextFile\Target.txt"); //Application.dataPath = AssetFolder
    public static StreamWriter RacketSpeedFilePath = File.AppendText(Application.dataPath + @"\TextFile\RacketSpeed.txt");
    public static StreamWriter RacketQuatFilePath = File.AppendText(Application.dataPath + @"\TextFile\RacketQuat.txt");
    public static StreamWriter RacketRotFilePath = File.AppendText(Application.dataPath + @"\TextFile\RacketRot.txt");
    public static StreamWriter FallPointFilePath = File.AppendText(Application.dataPath + @"\TextFile\FallPoint.txt");

    //ターゲットにHITしたとき
    public static void WriteHitTargetNum()
    {
        TargetFilePath.WriteLine("T,");
    }

    //ターゲットを外したとき
    public static void WriteMissTargetNum()
    {
        TargetFilePath.WriteLine("F,");
    }

    //ラケットスピード
    public static void WriteRacketSpeed(float speed)
    {
        RacketSpeedFilePath.WriteLine(speed.ToString()+",");
    }

    //ラケットの角度（クォータニオン）
    public static void WriteRacketQuat(Quaternion rot)
    {
        RacketQuatFilePath.WriteLine(rot.ToString() + ",");
        WriteRacketRot(rot.eulerAngles);
    }

    //ラケットの角度（Vector3）
    private static void WriteRacketRot(Vector3 rot)
    {
        RacketRotFilePath.WriteLine(rot.ToString() + ",");
    }

    //落下地点座標
    public static void WriteFallPoint(Vector3 point)
    {
        FallPointFilePath.WriteLine(point.ToString() + ",");
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
