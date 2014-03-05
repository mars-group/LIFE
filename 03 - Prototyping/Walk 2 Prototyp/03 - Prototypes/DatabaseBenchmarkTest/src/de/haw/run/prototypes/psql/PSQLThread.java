package de.haw.run.prototypes.psql;

import de.haw.run.prototypes.common.TestObject;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.UUID;
import java.util.concurrent.Callable;

/**
 * Project: RUN
 * User: chhuening
 * Date: 26.06.13
 * Time: 11:50
 */
public class PSQLThread implements Callable<Boolean> {

    private final Connection client;
    private int elements;
    private final int total_elements;

    public PSQLThread(Connection client, int elements){
        this.client = client;
        this.elements = elements;
        total_elements = elements;
    }


    @Override
    public Boolean call() {
        while(!Thread.currentThread().isInterrupted()){
            String s = UUID.randomUUID().toString();
            try {
                Statement st = null;
                ResultSet rs = null;
                st = client.createStatement();
                TestObject to = new TestObject(s, System.currentTimeMillis());
                long time = to.getTime();
                String value = to.getT1();
                UUID functionalId = to.getId();
                StringBuilder sb = new StringBuilder();


                for(int i = 1; i<=10; i++){
                    sb.append("INSERT INTO d_t");
                    sb.append(i);
                    sb.append(" (value, time, functionalId) VALUES ('");
                    sb.append(to.getT1());
                    sb.append("','");
                    sb.append(time);
                    sb.append("','");
                    sb.append(functionalId);
                    sb.append("');\n");
                }

                sb.append("INSERT INTO facts (f_fact, f_t1, f_t2, f_t3, f_t4, f_t5, f_t6, f_t7, f_t8, f_t9, f_t10) VALUES ('fact_value',\n");

                for(int i = 1; i<=10; i++){
                    sb.append("'");
                    sb.append(functionalId);
                    sb.append("',\n");
                }

                sb.deleteCharAt(sb.length()-2);
                sb.append(");");

                st.execute(sb.toString());

                elements--;

                if(elements == 0){
                    System.out.println("Thread " + Thread.currentThread().getName() + " created " + total_elements + " Elements.");
                    return true;
                }

            } catch (SQLException ex) {
                System.err.println(ex);
            }

        }
        return false;
    }
}
