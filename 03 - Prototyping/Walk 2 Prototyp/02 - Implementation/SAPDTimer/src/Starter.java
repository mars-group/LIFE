import de.haw.run.services.IGlobalClock;
import de.haw.run.services.globalclock.SAPDTimer;

import java.io.*;
import java.util.Scanner;

public class Starter {



    public static void main(String[] args) throws IOException, InterruptedException {
        Scanner scan = new Scanner(System.in);

        IGlobalClock sapdTimer = new SAPDTimer("test_host");
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
