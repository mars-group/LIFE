import de.haw.run.sapd.Implementation.SAPDTimer;
import de.haw.run.sapd.Interfaces.ISAPDTimer;

import java.io.*;
import java.util.Scanner;

public class Starter {



    public static void main(String[] args) throws IOException, InterruptedException {
        Scanner scan = new Scanner(System.in);

        ISAPDTimer sapdTimer = new SAPDTimer();
        sapdTimer.startTimer(true);

        System.out.println("Timer gestartet!");



        int i = 0;
        while(i < 10){
            //Thread.sleep(10);
            System.out.println(sapdTimer.getTime());
            i++;
        }


        sapdTimer.stopTimer();
    }

}
