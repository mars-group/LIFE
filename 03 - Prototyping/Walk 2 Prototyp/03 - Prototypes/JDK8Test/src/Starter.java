import java.util.*;
import java.util.stream.Collectors;

public class Starter {

    public static void main(String[] args){
        List<String> ss = new LinkedList<>();
        for(int i=0; i<1000000; i++){
            ss.add("x,y,z");
            ss.add("x.y.z");
            ss.add("a,b,c");
            ss.add("i,j,k");
        }
        //System.out.println(ss.size());

        long start = System.currentTimeMillis();
        String result = ss.stream()
                .map(s -> s.replace(',','.'))
                .sorted(Comparators.naturalOrder())
                .collect(Collectors.toStringJoiner(","))
                .toString();
        long end = System.currentTimeMillis();
        System.out.println("serial: " + (end-start));

        long start2 = System.currentTimeMillis();
        String res = ss.stream().parallel()
                .map(s -> s.replace(',', '.'))
                .sorted(Comparators.naturalOrder())
                .collect(Collectors.toStringJoiner(","))
                .toString();
        long end2 = System.currentTimeMillis();
        System.out.println("parallel: " + (end2-start2));
    }

}
