package de.haw.run.services.globalclock;

import com.ericsson.otp.erlang.*;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.services.IGlobalClock;
import de.haw.run.services.IAlarmClock;

import java.io.*;

/**
 * User: Christian Hüning
 * Date: 15.08.13
 */


public class SAPDTimer implements IGlobalClock {


    static String SourcePath;
    private String timerHostAddress;
    private String timerNodeName;
    private String cookie;

    private Process sAPDProc;
    private OtpNode node;
    private OtpMbox mbox;

    public SAPDTimer(String nodename){
        AppSettings appSettings = new AppSettings();


        try {
            SourcePath = (new File(".").getCanonicalPath() + appSettings.getString("ErlangBeamFilesRelativePath")).replaceAll("\\\\","/");

            cookie = appSettings.getString("CookieName");
            timerNodeName = nodename + "_" + appSettings.getString("TimerNodeNameBase");

            node = new OtpNode(nodename + "_" + appSettings.getString("ClientNodeNameBase"), cookie);

            timerHostAddress = timerNodeName + "@" + node.host();

            mbox = node.createMbox("pdc_mbox");
        } catch (IOException | SettingException e) {
            e.printStackTrace();
        }

    }


    public Thread setTimer(final long wakeUpTime, IAlarmClock wakeMeUp){

        // TODO: This should maybe be changed from actively waiting to subscribing with SAPD_Controller

        Thread t = new Thread(new Runnable() {

            @Override
            public void run() {
                long currentTime = -1;

                while(currentTime < wakeUpTime){
                    currentTime = getTime();
                }

                wakeMeUp.wakeUp();
            }

        });

        t.start();

        return t;
    }


    @Override
    public long getTime() {
        try {


            OtpErlangObject[] msg = new OtpErlangObject[2];
            msg[0] = new OtpErlangAtom("getPDC");
            msg[1] = mbox.self();
            OtpErlangTuple tuple = new OtpErlangTuple(msg);

            mbox.send("sapd_controller", timerHostAddress, tuple);

            OtpErlangObject reply = mbox.receive();

            if(reply instanceof OtpErlangTuple){
                OtpErlangTuple res = (OtpErlangTuple)reply;
                OtpErlangLong pdcCounter = (OtpErlangLong)(res.elementAt(0));
                return pdcCounter.longValue();
            }
            return -1;

        } catch (OtpErlangDecodeException | OtpErlangExit e) {
            e.printStackTrace();
        }
        return -1;
    }

    @Override
    public void startTimer(boolean sponsor) {
        String module;
        if(sponsor){
            module = "zptc_sponsor";
        } else {
            module = "zptc";
        }


        ProcessBuilder pb = new ProcessBuilder("erl",
                "-sname", timerNodeName,
                "-setcookie", cookie,
                "-pa",  SourcePath,
                "-run", module,
                "");

        pb.redirectErrorStream(true);

        try {
            sAPDProc = pb.start();
            if(sponsor){
                Thread.sleep(10000);
            }
        } catch (IOException | InterruptedException e) {
            e.printStackTrace();
        }


    }

    @Override
    public void stopTimer() {
        if(sAPDProc != null){
            sAPDProc.destroy();
        }
    }

}
