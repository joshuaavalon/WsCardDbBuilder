package database;

import java.util.ArrayList;
import java.util.List;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.DataOutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLConnection;

import java.sql.Connection;
import java.sql.DatabaseMetaData;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.sql.Statement;

import java.sql.PreparedStatement;

import java.sql.ResultSet;

public class DataBase {

    List<String> allSer = new ArrayList<String>();
    List<Integer> serPage = new ArrayList<Integer>();
    
    public static void main(String[] args) throws IOException {

        DataBase d = new DataBase();
        
        d.createDataBase();
        //d.readDataBase();
        //d.grabIDAndPage();
        //d.grabAllCardID();
        //d.connectToCard("DC/W01-003");
        
        
    }
    public void createDataBase() throws IOException{
        String directry = "jdbc:sqlite:C:/sqlite/db/test.db";
        
        String sql = "CREATE TABLE IF NOT EXISTS WS (\n"
                + "	カード番号 text NOT NULL,\n"
                + "	カード名 text NOT NULL,\n"
                + "	エクスパンション text NOT NULL,\n"
                + "	サイド text NOT NULL,\n"
                + "	レアリティ text NOT NULL,\n"
                + "	種類 text NOT NULL,\n"
                + "	色 text NOT NULL,\n"
                + "	レベル text NOT NULL,\n"
                + "	コスト text NOT NULL,\n"
                + "	パワー text NOT NULL,\n"
                + "	ソウル text NOT NULL,\n"
                + "	トリガー text NOT NULL,\n"
                + "	特徴 text NOT NULL,\n"
                + "	テキスト text NOT NULL,\n"
                + "	フレーバー text NOT NULL,\n"
                + "	capacity real\n"
                + ");";
        
        
        try (Connection conn = DriverManager.getConnection(directry)) {
            
            //Create .db if not exist
            if (conn != null) {
                DatabaseMetaData meta = conn.getMetaData();
                System.out.println("The driver name is " + meta.getDriverName());
                System.out.println("A new database has been created.");
            }
            
            try (Statement stmt = conn.createStatement()) {
            // create a new table
                stmt.execute(sql);
            } catch (SQLException e) {
                System.out.println(e.getMessage());
            }
        } catch (SQLException e) {
            System.out.println(e.getMessage());
        }
    }
    
    public void readDataBase() throws IOException{
        
        String directry = "jdbc:sqlite:C:/sqlite/db/test.db";
        String reading = "SELECT カード番号,カード名,エクスパンション,サイド,レアリティ,種類,色,レベル,コスト,パワー,ソウル,トリガー,特徴,テキスト,フレーバー,capacity FROM WS";
            
        try (Connection conn = DriverManager.getConnection(directry); Statement stmt  = conn.createStatement();
             ResultSet rs    = stmt.executeQuery(reading)){
            
            // loop through the result set
            while (rs.next()) {
                System.out.println(rs.getString("カード番号") +  "\t" + 
//                                   rs.getString("カード名") + "\t" +
//                                   rs.getString("エクスパンション") + "\t" +
//                                   rs.getString("サイド") + "\t" +
//                                   rs.getString("レアリティ") + "\t" +
//                                   rs.getString("種類") + "\t" +
//                                   rs.getString("色") + "\t" +
//                                   rs.getString("レベル") + "\t" +
//                                   rs.getString("コスト") + "\t" +
//                                   rs.getString("パワー") + "\t" +
//                                   rs.getString("ソウル") + "\t" +
//                                   rs.getString("トリガー") + "\t" +
//                                   rs.getString("特徴") + "\t" +
//                                   rs.getString("テキスト") + "\t" +
                                   rs.getString("フレーバー") + "\t" +
                                   rs.getDouble("capacity"));
            }
        } catch (SQLException e) {
            System.out.println(e.getMessage());
        }
    }
     public static void insertData(String no, String name, String series, String side, String rarity, String type, String color, String level, String cost, String power, String soul, String trigger, String characteristic, String text, String flavor, String cap) throws IOException{
        
            String directry = "jdbc:sqlite:C:/sqlite/db/test.db";
            String insertNew = "INSERT INTO WS(カード番号,カード名,エクスパンション,サイド,レアリティ,種類,色,レベル,コスト,パワー,ソウル,トリガー,特徴,テキスト,フレーバー,capacity) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
            try (Connection conn = DriverManager.getConnection(directry);
                    PreparedStatement pstmt = conn.prepareStatement(insertNew)) {
            pstmt.setString(1, no);
            pstmt.setString(2, name);
            pstmt.setString(3, series);
            pstmt.setString(4, side);
            pstmt.setString(5, rarity);
            pstmt.setString(6, type);
            pstmt.setString(7, color);
            pstmt.setString(8, level);
            pstmt.setString(9, cost);
            pstmt.setString(10, power);
            pstmt.setString(11, soul);
            pstmt.setString(12, trigger);
            pstmt.setString(13, characteristic);
            pstmt.setString(14, text);
            pstmt.setString(15, flavor);
            pstmt.setString(16, cap);
            pstmt.executeUpdate();
        } catch (SQLException e) {
            System.out.println(e.getMessage());
        }
     }
     
     
     public void grabIDAndPage() throws IOException{
        String line;
        
        String USER_AGENT = "Mozilla/5.0";
        
        URL initialurl = new URL("http://ws-tcg.com/cardlist/");
        
        InputStream initialis =initialurl.openStream();
        BufferedReader initialbr =new BufferedReader(new InputStreamReader(initialis));
        
        String tem = "";
        int st = 0;
        while ((line = initialbr.readLine()) != null) {
            //System.out.println(line);
            if(line.startsWith("<li><a href=\"java"))
            {
                st = line.indexOf("'");
                st++;
                while(line.charAt(st) != '\'')
                {
                    tem = tem + line.charAt(st);
                    st++;
                }
                allSer.add(tem);
                tem = "";
            }
        }
        
        URL url = new URL("http://ws-tcg.com/jsp/cardlist/expansionDetail");
        InputStream is =url.openStream();
        
        for(int i = 0; i < allSer.size()-1;i++)
        {
            HttpURLConnection con = (HttpURLConnection) url.openConnection();

        con.setRequestMethod("POST");
        
        con.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
        
            String urlParameters = "expansion_id="+ allSer.get(i) +"&page=";
        
            con.setDoOutput(true);
		DataOutputStream wr = new DataOutputStream(con.getOutputStream());
		wr.writeBytes(urlParameters);
		wr.flush();
		wr.close();
        
        
            BufferedReader in = new BufferedReader(
		        new InputStreamReader(con.getInputStream()));
		String inputLine;
		StringBuffer response = new StringBuffer();

                int pagenumber;
                String temPageNumber = "";
		while ((inputLine = in.readLine()) != null) {
                    //System.out.println(inputLine);
                    if(inputLine.startsWith("<span class='disable'>≪</span> <strong>"))
                    {
                        pagenumber = inputLine.length() - 85 - allSer.get(i).length();
                        if(!Character.isDigit(inputLine.charAt(pagenumber)))
                        {
                            pagenumber++;
                        }
                        while(Character.isDigit(inputLine.charAt(pagenumber)))
                        {
                            temPageNumber = temPageNumber + inputLine.charAt(pagenumber);
                            pagenumber++;
                        }
                    }
                    response.append(inputLine);
		}
                if(temPageNumber.isEmpty())
                {
                    temPageNumber = "1";
                }
                serPage.add(Integer.valueOf(temPageNumber));
                temPageNumber = "";
            in.close();
        }
     }
     public void grabAllCardID() throws IOException{
         URL url = new URL("http://ws-tcg.com/jsp/cardlist/expansionDetail");
        InputStream is =url.openStream();
        
        for(int i = 0; i < allSer.size(); i++)
        //for(int i = 0; i < 1; i++)
        {
            for(int j = 1; j < serPage.get(i) + 1; j++)
            {
                
            
            HttpURLConnection con = (HttpURLConnection) url.openConnection();

            con.setRequestMethod("POST");
            con.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
            
            //String urlParameters = "expansion_id=1&page=9";
            //String urlParameters = "expansion_id="+ allSer.get(0) +"&page=1";
            String urlParameters = "expansion_id="+ allSer.get(i) +"&page=" + String.valueOf(j);
        
            con.setDoOutput(true);
            DataOutputStream wr = new DataOutputStream(con.getOutputStream());
            wr.writeBytes(urlParameters);
            wr.flush();
            wr.close();
        
        
            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
            String inputLine;
            StringBuffer response = new StringBuffer();

            
            String temID = "";
            int position;
            while ((inputLine = in.readLine()) != null) {
                //System.out.println(inputLine);
                inputLine = inputLine.replaceAll("\\s+","");

                if(inputLine.startsWith("<td><ahref=\"?cardno="))
                    //if(inputLine.startsWith("<a href=\"javascript:void(0);\" onclick"))
                    {
                        position = 20;
                        //System.out.println(inputLine.charAt(position));
                        while(inputLine.charAt(position) != '\"')
                        {
                            temID = temID + inputLine.charAt(position);
                            position++;
                        }
                        /////////////////////////////////////////////////////////////////////////////////////////////
                        
                        connectToCard(temID);
                        
                        temID = "";
                    }
                response.append(inputLine);
            }
            
            in.close();
        }
        }
    }
     
     
     
     public void connectToCard(String cardID) throws IOException{
         
        String line;
        String temName = "";
        String temRarity = "";
        String temSeries = "";
        String temSide = "";
        String temType = "";
        String temColor = "";
        String temLevel = "";
        String temCost = "";
        String temPower = "";
        String temSoul = "";
        String temTrigger = "";
        String temCharacteristic = "";
        String temText = "";
        String temFlavor = "";
        
        
        URL cardUrl = new URL("http://ws-tcg.com/cardlist/?cardno=" + cardID);
         
         
        
        InputStream initialis =cardUrl.openStream();
        BufferedReader initialbr =new BufferedReader(new InputStreamReader(initialis));
        
        String tem = "";
        int counter;
        
        int ready = 0;
        int st;
        while ((line = initialbr.readLine()) != null) {
            ready--;
            //System.out.println(line);
            if(temName.isEmpty() && line.contains("カード名"))
            {
                ready = 2;
            }
            if(temRarity.isEmpty() && line.contains("cell_4"))
            {
                st = line.indexOf(">");
                st++;
                while(line.charAt(st) != '<')
                    {
                        tem = tem + line.charAt(st);
                        st++;
                    }
                temRarity = tem;
                tem = "";
            }
            if(temSeries.isEmpty() && line.contains("<th>エクスパンション</th>"))
            {
                ready = 1;
            }
            if(temSide.isEmpty() && line.contains("<th>サイド</th>"))
            {
                ready = 2;
            }
            if(temType.isEmpty() && line.contains("<th>種類</th>"))
            {
                ready = 1;
            }
            if(temColor.isEmpty() && line.contains("<th>色</th>"))
            {
                ready = 1;
            }
            if(temLevel.isEmpty() && line.contains("<th>レベル</th>"))
            {
                ready = 1;
            }
            if(temCost.isEmpty() && line.contains("<th>コスト</th>"))
            {
                ready = 1;
            }
            if(temPower.isEmpty() && line.contains("<th>パワー</th>"))
            {
                ready = 1;
            }
            if(temSoul.isEmpty() && line.contains("<th>ソウル</th>"))
            {
                ready = 1;
            }
            if(temTrigger.isEmpty() && line.contains("<th>トリガー</th>"))
            {
                ready = 1;
            }
            if(temCharacteristic.isEmpty() && line.contains("<th>特徴</th>"))
            {
                ready = 2;
            }
            if(temText.isEmpty() && line.contains("<th>テキスト</th>"))
            {
                ready = 1;
            }
            if(temFlavor.isEmpty() && line.contains("<th>フレーバー</th>"))
            {
                ready = 1;
            }
            
            
            
            if(ready == 0)
            {
                if(temName.isEmpty())
                {
                    st = 0;
                    while(line.charAt(st) != '<')
                    {
                        tem = tem + line.charAt(st);
                        st++;
                    }
                    temName = tem;
                    tem = "";
                }
                else 
                {
                    if(temSeries.isEmpty())
                    {
                        st = line.indexOf(">");
                        st++;
                        while(line.charAt(st) != '<')
                        {
                            tem = tem + line.charAt(st);
                            st++;
                        }
                        temSeries = tem;
                        tem = "";
                    }
                    else 
                    {
                        if(temSide.isEmpty())
                        {
                            st = line.indexOf("gif");
                            st = st - 2;
                            temSide = temSide + line.charAt(st);
                            
                            
                        }
                        else
                        {
                            if(temType.isEmpty())
                            {
                                st = line.indexOf(">");
                                st++;
                                while(line.charAt(st) != '<')
                                {
                                    tem = tem + line.charAt(st);
                                    st++;
                                }
                                temType = tem;
                                tem = "";
                                
                            }
                            else
                            {
                                if(temColor.isEmpty())
                                {
                                    st = line.indexOf("partimages");
                                    st = st + 11;
                                    while(line.charAt(st) != '.')
                                    {
                                        tem = tem + line.charAt(st);
                                        st++;
                                    }
                                    temColor = tem;
                                    tem = "";
                                    
                                }
                                else
                                {
                                    if(temLevel.isEmpty())
                                    {
                                        st = line.indexOf(">");
                                        st++;
                                        while(line.charAt(st) != '<')
                                        {
                                            tem = tem + line.charAt(st);
                                            st++;
                                        }
                                        temLevel = tem;
                                        tem = "";
                                    }
                                    else
                                    {
                                        if(temCost.isEmpty())
                                        {
                                            st = line.indexOf(">");
                                            st++;
                                            while(line.charAt(st) != '<')
                                            {
                                                tem = tem + line.charAt(st);
                                                st++;
                                            }
                                            temCost = tem;
                                            tem = "";
                                        }
                                        else
                                        {
                                            if(temPower.isEmpty())
                                            {
                                                st = line.indexOf(">");
                                                st++;
                                                while(line.charAt(st) != '<')
                                                {
                                                    tem = tem + line.charAt(st);
                                                    st++;
                                                }
                                                temPower = tem;
                                                tem = "";
                                            }
                                            else
                                            {
                                                if(temSoul.isEmpty())
                                                {
                                                    st = 0;
                                                    counter = 0;
                                                    while(st != -1)
                                                    {
                                                        st = line.indexOf("soul");
                                                        if(st != -1)
                                                        {
                                                            counter++;
                                                            line = line.substring(st+4);
                                                        }
                                                    }
                                                    temSoul = Integer.toString(counter);
                                                }
                                                else
                                                {
                                                    if(temTrigger.isEmpty())
                                                    {
                                                        st = 0;
                                                        tem = "";
                                                        while(st != -1)
                                                        {
                                                            st = line.indexOf("partimages");
                                                            if(st != -1)
                                                            {
                                                                if(!tem.isEmpty())
                                                                {
                                                                    tem = tem + ",";
                                                                }
                                                                st = st + 11;
                                                                while(line.charAt(st) != '.')
                                                                {
                                                                    tem = tem + line.charAt(st);
                                                                    st++;
                                                                }
                                                                line = line.substring(st);
                                                            }
                                                            
                                                        }
                                                        temTrigger = tem;
                                                        tem = "";
                                                        if(temTrigger.isEmpty())
                                                        {
                                                            temTrigger = "-";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(temCharacteristic.isEmpty())
                                                        {
                                                            temCharacteristic = line;
                                                        }
                                                        else
                                                        {
                                                            if(temText.isEmpty())
                                                            {
                                                                temText = line.substring(0, line.length() - 11).substring(16);
                                                                temText = temText.replace("<br />", "\n");
                                                                if(temText.contains("img"))
                                                                {
                                                                    temText = temText.replace("<img src='../cardlist/partimages/", "(");
                                                                    temText = temText.replace(".gif' />", ")");
                                                                }
                                                                
                                                            }
                                                            else
                                                            {
                                                                if(temFlavor.isEmpty())
                                                                {
                                                                    temFlavor = line.substring(0, line.length() - 11).substring(16);
                                                                    temFlavor = temFlavor.replace("<br />", "\n");
                                                                    if(temFlavor.contains("img"))
                                                                    {
                                                                        temFlavor = temFlavor.replace("<img src='../cardlist/partimages/", "(");
                                                                        temFlavor = temFlavor.replace(".gif' />", ")");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                
            }
            
        }

        //System.out.println(temText);
        insertData(cardID, temName, temSeries, temSide, temRarity, temType, temColor, temLevel, temCost, temPower, temSoul, temTrigger, temCharacteristic, temText, temFlavor, "4");
    }
}


