package de.janbusch.voltdb.benchmark.storedprocs;

import org.voltdb.SQLStmt;
import org.voltdb.VoltProcedure;
import org.voltdb.VoltTable;

public class InsertDimensionX extends VoltProcedure {
	public final SQLStmt sql = new SQLStmt("INSERT INTO ? VALUES (?, ?, ?);");

	public VoltTable[] run(int dId, String tableName, String value, String time)
			throws VoltAbortException {
		voltQueueSQL(sql, tableName, dId, value, time);
		voltExecuteSQL();

		return null;
	}
}
