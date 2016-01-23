package de.haw.run.prototypes.mysql;

import com.mysql.jdbc.*;
import de.haw.run.prototypes.common.TestObject;

import java.sql.*;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.Statement;
import java.util.UUID;
import java.util.concurrent.Callable;

/**
 * Project: RUN
 * User: chhuening
 * Date: 26.06.13
 * Time: 11:50
 */
public class MySQLThread implements Callable<Boolean> {

    private final Connection client;
    private int elements;
    private final int total_elements;

    public MySQLThread(Connection client, int elements){
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


                for(int i = 1; i<=10; i++){
                    StringBuilder sb = new StringBuilder();
                    sb.append("INSERT INTO ");
                    sb.append("datacube_test_ch");
                    sb.append(".d_t");
                    sb.append(i);
                    sb.append("(value, time, functionalId) VALUES('");
//                    pstmt = client.prepareStatement(sb.toString());
//                    pstmt.setString(1, to.getT1());
//                    pstmt.setString(2, String.valueOf(time));
//                    pstmt.setString(3, functionalId.toString());
//                    pstmt.addBatch();

                    sb.append(value);
                    sb.append("','");
                    sb.append(time);
                    sb.append("','");
                    sb.append(functionalId);
                    sb.append("');\n");
                    st.addBatch(sb.toString());
                }


                StringBuilder sb = new StringBuilder();
                sb.append("INSERT INTO ");
                sb.append("datacube_test_ch");
                sb.append(".facts (f_fact, f_t1, f_t2, f_t3, f_t4, f_t5, f_t6, f_t7, f_t8, f_t9, f_t10) VALUES ('fact_value',\n");

                for(int i = 1; i<=10; i++){
                    sb.append("'");
                    sb.append(functionalId);
                    sb.append("',\n");
                }

                sb.deleteCharAt(sb.length()-2);
                sb.append(");");

                st.addBatch(sb.toString());

                st.executeBatch();

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
