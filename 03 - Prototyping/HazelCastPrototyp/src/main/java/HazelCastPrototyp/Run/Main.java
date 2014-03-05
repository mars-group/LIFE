package HazelCastPrototyp.Run;

import HazelCastPrototyp.Impl.Client;
import HazelCastPrototyp.Impl.SuperHero;
import com.hazelcast.core.IExecutorService;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.List;

/**
 * Created with IntelliJ IDEA.
 * User: Nils
 * Date: 04.11.13
 * Time: 12:55
 * To change this template use File | Settings | File Templates.
 */
public class Main {

    private static BufferedReader console = new BufferedReader(new InputStreamReader(System.in));

    private static String input = "";

    private static Client client = new Client();

    private static List<SuperHero> superherosList = client.getClient().getList("SuperHeroes");

    public static void main(String[] args) {

        try {
            while (! input.equals("quit")) {
                printMenu();
                input = console.readLine().trim();

                if (input.equals("1")) {
                    readName();
                }

                if (input.equals("2")) {
                    System.out.println("please enter the name");
                    input = console.readLine();
                    getHeroByName(superherosList, input);
                }

                if (input.equals("3")) {
                    superherosList = deleteHeroByName(superherosList, input);
                }

                if (input.equals("4")) {
                    for (SuperHero hero : superherosList) {
                        System.out.println(hero);
                    }
                }

                if (input.equals("7")) {
                    System.out.println(client.getClient().getCluster().getMembers().size());
                }

                if (input.equals("8")) {
                    System.out.println(superherosList.size());
                }

                if(input.equals("9")){
                    long start = System.currentTimeMillis();
                    for (int i = 0; i < 100000; i++) {
                        superherosList.add(new SuperHero("GenericSuperHero<T extends SuperPower>" + i));
                    }
                    long end = System.currentTimeMillis();
                    System.out.println("Duration: " + (end-start) + "ms, " + (end-start)/1000 + "secs, " + (end-start)/1000/60.0 + "mins.");
                }

                if(input.equals("10")){

                    IExecutorService es = client.getClient().getExecutorService("default");

                    long start = System.currentTimeMillis();
                    es.submitToMembers(() -> {
                        for (int i = 0; i < 100000; i++) {
                            superherosList.add(new SuperHero("GenericSuperHero<T extends SuperPower>" + i));
                        }
                        return true;
                    }, client.getClient().getCluster().getMembers());


                    long end = System.currentTimeMillis();
                    System.out.println("Duration: " + (end-start) + "ms, " + (end-start)/1000 + "secs, " + (end-start)/1000/60.0 + "mins.");
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
        System.out.println("bye");

    }

    private static void readName() {
        System.out.println("What's your new super hero name?");
        try {
            input = console.readLine();
            superherosList.add(new SuperHero(input));
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private static void printMenu() {
        System.out.println("1. add new Hero");
        System.out.println("2. get Hero by name ");
        System.out.println("3. delete Hero by name");
        System.out.println("4. view List");
        System.out.println("7. Print member count.");
        System.out.println("8. Print size of list.");
        System.out.println("9. Add 100.000 GenericSuperPowers");
        System.out.println("10. Add 100.000 GenericSuperPowers via DistributedTask");
    }

    private static List<SuperHero> deleteHeroByName(List<SuperHero> superherosList, String input) {
        for (int i = 0; i < superherosList.size(); i++) {
            if (superherosList.get(i).getName() == input) superherosList.remove(i);
        }
        return superherosList;
    }

    private static void getHeroByName(List<SuperHero> list, String name) {
        try {
            input = console.readLine().trim();
            for (SuperHero hero : list) {
                if (hero.getName() == name)
                    System.out.println("Your hero is " + hero);
            }
            System.out.println("cant find hero");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
