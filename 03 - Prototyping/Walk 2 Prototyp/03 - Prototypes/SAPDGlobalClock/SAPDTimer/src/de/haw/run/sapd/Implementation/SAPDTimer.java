package de.haw.run.sapd.Implementation;

import com.ericsson.otp.erlang.*;
import de.haw.run.sapd.Interfaces.ISAPDTimer;

import java.io.*;

/**
 * User: Christian Hüning
 * Date: 15.08.13
 */


public class SAPDTimer implements ISAPDTimer {


    static final String SourcePath = "\"F:/SoftwareProjekte/Run/03 - Prototypes/SAPDGlobalClock/out/production/SAPD Erlang\"";

    private Process sAPDProc;
    private OtpNode node;
    private OtpMbox mbox;

    public SAPDTimer(){
        try {
            node = new OtpNode("chhaw_client", "zummsel");
            mbox = node.createMbox("pdc_mbox");
        } catch (IOException e) {
            e.printStackTrace();
        }

    }

    @Override
    public long getTime() {
        try {


            OtpErlangObject[] msg = new OtpErlangObject[2];
            msg[0] = new OtpErlangAtom("getPDC");
            msg[1] = mbox.self();
            OtpErlangTuple tuple = new OtpErlangTuple(msg);

            mbox.send("sapd_controller", "chhaw2@chdesktop", tuple);

            OtpErlangObject reply = mbox.receive();

            if(reply instanceof OtpErlangTuple){
                OtpErlangTuple res = (OtpErlangTuple)reply;
                OtpErlangLong pdcCounter = (OtpErlangLong)(res.elementAt(0));
                return pdcCounter.longValue();
            }
            return -1;

        } catch (OtpErlangDecodeException e) {
            e.printStackTrace();
        } catch (OtpErlangExit otpErlangExit) {
            otpErlangExit.printStackTrace();
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
        try {
            System.out.println(new File(".").getCanonicalPath());
        } catch (IOException e) {
            e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
        }
        ProcessBuilder pb = new ProcessBuilder("erl",
                "-sname", "chhaw2",
                "-setcookie", "zummsel",
                "-pa",  SourcePath,
                "-run", module,
//                "-init_debug");
                "");

        pb.redirectErrorStream(true);

        try {
            sAPDProc = pb.start();
            Thread.sleep(10000);
/*
            OutputStream stdin = sAPDProc.getOutputStream ();
            InputStream stdout = sAPDProc.getInputStream ();

            BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(stdin));
            BufferedReader reader = new BufferedReader(new InputStreamReader(stdout));


            writer.write("zptc_sponsor:start().\n");

            writer.flush();

            while(reader.readLine() != null){
                System.out.println(reader.readLine());
            }
  */
        } catch (IOException e) {
            e.printStackTrace();
        } catch (InterruptedException e) {
            e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
        }


    }

    @Override
    public void stopTimer() {
        if(sAPDProc != null){
            sAPDProc.destroy();
        }
    }

}
