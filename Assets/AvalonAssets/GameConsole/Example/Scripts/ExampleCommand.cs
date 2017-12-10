using System;
using AvalonAssets.Unity;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Example
{
    public class ExampleCommand : MonoBehaviour
    {
        public void Madoka(Action<string> output, string[] args)
        {
            output.Invoke("　　　　　　　　　　　　 ∧ .　-‐. : . : . : . : . ￣｀丶/＞ﾍ',ｰ.､");
            output.Invoke(".　　　　　　　　 __　　/／: . : . : . : . : . : . : . : . : . : ＼: . }i: : :＼");
            output.Invoke("　　　　　　　　 }l＞／: . : . : . : . : . : . : . : . : . : . : . : :.＼}i: . : . :＼__");
            output.Invoke("　　　　　　　／}ﾚ': : . : . : . : . : . : . : . : . : .＼: . ヽ: . : . :.ヽ: . : . : . ヽ｀");
            output.Invoke("　　　　　　 /: :/: . : . : . : . :l: :八: . : . : : ＼: : : : : '. : . : . :ｌ: . : . : l: : l");
            output.Invoke(".　　　　　 /: :/: . : . : . : . :/l: :|　＼: :＼: : : ＼: : . :l: . : :...:|: . : . : l: . l");
            output.Invoke("　　　　　/: ://: : . : . :.|: / .j: :|　 　＼: ヽ＼:ヽ＼: :|: . : |: :|: : |: . :l: : |");
            output.Invoke("　　　　 //://|: : :ｌ: : . i斗'^　　　　 　 　,ｨf弌ぅ ､ヽ|: . : l: :|: : |: . :|＼|");
            output.Invoke("　　　／/:ノ /|: :/}: : : |: l ,ｨfﾟう　　　　　 　ﾄ::)ﾟｨﾘヽ|: . : |:/: : :|: . :|");
            output.Invoke("　　　　 }: : /|:|:/: |: : ∧:i/ん:::ﾊ　　　　 　弋c少'　|: . : |｝: : :.|: . :|");
            output.Invoke("　　　　 |: / .|:|': : |: :/: :i　弋cソ　 　 　 　 ､､､､､.　|: . :八: . :八: .|");
            output.Invoke("　　　　 |/　 l: : : :|V: : ﾊ　 ､､､､　　′　　 　 　 　 ｌ: ./:∧: ./ 　ヽ");
            output.Invoke("　　　　　　 八: :.∧: |: :人　 　r ､_　 っ　　　　 ｨ .ﾉ:/:/　}:./");
            output.Invoke("　　　　　　　　∨　V}: : |: /i＞}/ ,へ、　　 イ j:V／:/　／");
            output.Invoke("　　　 　 　 　 　 　 八: :|/＿_ !　　/ ﾊ　＿.斗‐{");
            output.Invoke("　　　　　 　 　 　 　　∧!　 ＿|　 　 ' /´　 　 　》──--､");
            output.Invoke("　 　 　　　　　　　　_/　 　 |:::_}　　　.|ー─--＜　　 　 　 |");
            output.Invoke("　　　 　　　　　　／___　 i　}∧　　　ﾉ:::::::::::::::::::/ ／ 　 　 |");
            output.Invoke("　　　　　　　　　 V　　　 ／　 }. 　ｲ l＼::::::::::::/ /　　 　　:|");
            output.Invoke("　　　　　　　　　rﾍ 　 xﾍ　 　 ∨　| |:|　＼／_/　　　 　 人");
            output.Invoke("　　　　　　　　 /＿／ 　 '、　　_ヽ | |:|　　 ./　 　- ─ 　 - 〉");
            output.Invoke(".　　　　　　　 /　　 　 　　 ヽ／/／ j/　　〈 ／　-‐　　　／");
        }

        public void Print(Action<string> output, string[] args)
        {
            output.Invoke("Printing all arguments:");
            foreach (var argument in args)
            {
                output.Invoke(argument);
            }
        }

        public void ColorHelloWorld(Action<string> output, string[] args)
        {
            output.Invoke("Hello World!".AddColor(Color.red));
        }
    }
}