using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compAssistant
{
   public class Querys
    {
       string grantNumber;
       public  List<string> getAllQuerys()
       {
           List<string> listOfQuerys = new List<string>();
           String accounts = getAccount();
           String funds = getFunds();
           String Orgs = getOrgs();
           String Locs = getLocations();
           String actv = getActivity();
           String prog = getPrograms();
           String Approvers = getApprovers();
           String Employees = GetEmployeesInformation();
           
           
           listOfQuerys.Add( funds );
           listOfQuerys.Add( accounts );
           listOfQuerys.Add( Orgs );
           listOfQuerys.Add( Locs );
           listOfQuerys.Add( actv );
           listOfQuerys.Add( prog );
           listOfQuerys.Add(Approvers);
           listOfQuerys.Add(Employees);
           return listOfQuerys;
       }

       public List<string> getAllGrantBillQueries()
       {
           List<string> listOfGrantQueries = new List<string>();

           String GrantAccountant = GetGrantAccounts();
           String ListGrants = GetGrants();
           //String CumulativeAmountBudget = CumeAmountBudget(grantNumber); //we will have to change this.
           //string CumeAmountActual = CumeAmountYTD(grantNumber);
           string listOfGrantsQuery = getListOfGrants();

           listOfGrantQueries.Add(GrantAccountant);
           listOfGrantQueries.Add(ListGrants);
          // listOfGrantQueries.Add(CumulativeAmountBudget);
           //listOfGrantQueries.Add(CumeAmountActual);
           //get current amount.
           return listOfGrantQueries;

       }
       public string FullRefresh()
       {
string sb =
"SELECT  distinct " +
" frrgenb_bfrm_code, " +
"       frrgenb_grnt_code, " +
"       frrgenb_bill_inv_seq_no, " +
"       frrgenb_bill_inv_adj_no, "  +
"       TO_CHAR(FRRGENB_PERIOD_TO_DATE,'DD-MON-RR') FRRGENB_PERIOD_TO_DATE, TO_CHAR(FRRGENB_PERIOD_FROM_DATE,'DD-MON-RR') FRRGENB_PERIOD_FROM_DATE" +
"       FROM  frrgenb ";
return sb;

       }

       public string getListOfGrants()
       {

           string queryForGrants =
 "WITH CTE AS                                                                                                                           " +
"(                                                                                                                                     " +
"   SELECT                                                                                                                             " +
"       ROW_NUMBER() OVER (PARTITION BY frrgenb_grnt_code ORDER BY frrgenb_bill_inv_seq_no  DESC) AS RN,        " +
"       frrgenb_bfrm_code,                                                                                                             " +
"       frrgenb_grnt_code,                                                                                                             " +
"       frrgenb_bill_inv_seq_no,                                                                                                       " +
"       frrgenb_bill_inv_adj_no , TO_CHAR(FRRGENB_PERIOD_TO_DATE,'DD-MON-RR') FRRGENB_PERIOD_TO_DATE, TO_CHAR(FRRGENB_PERIOD_FROM_DATE,'DD-MON-RR') FRRGENB_PERIOD_FROM_DATE                                                                                                       " +
"   FROM                                                                                                                               " +
"      frrgenb WHERE FRRGENB_COMPLETE_IND = 'Y'                                                                                        " +
")                                                                                                                                     " +
"SELECT                                                                                                                                " +
" frrgenb_bfrm_code,                                                                                                                   " +
"       frrgenb_grnt_code,                                                                                                             " +
"       frrgenb_bill_inv_seq_no,                                                                                                       " +
"       frrgenb_bill_inv_adj_no   , FRRGENB_PERIOD_TO_DATE,    FRRGENB_PERIOD_FROM_DATE                                                " +
"                                                                                                                                      " +
"FROM CTE                                                                                                                              " +
"WHERE RN = 1 /*and  frrgenb_grnt_code = '270943'  */          ";
           return queryForGrants;

       }

       public string CumeAmountBudget(String GrantNumber, string transdate)
       {



           string CumeAmount = @"SELECT '1', NVL(sum(frvgtrd_trans_amt),0)  Total " +
"From frvgtrd f " +
"where frvgtrd_GRNT_code = '" + GrantNumber + "'" +
" and   frvgtrd_ACCT_CODE  between '2000' and '2899' " +
"and frvgtrd_Acct_code not in " +
"(" +
"with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '2%' and frrbecl_coas_code = 'D' " +
"    union all" +
"    select accounts + 1, ACCT_TO" +
"    from excludeAccounts" +
"    where accounts + 1 <= ACCT_TO" +
")" +
"select accounts " +
"from  excludeAccounts" +
")" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC in('ABD', 'OBD') " +
" union" +
" (SELECT '2', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd" +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   frvgtrd_ACCT_CODE  between '2900' and '2999' " +
" and frvgtrd_Acct_code not in " +
"(" +
" with excludeAccounts (accounts, ACCT_TO) as ( " +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '2%' and frrbecl_coas_code = 'D' " +
"    union all" +
"    select accounts + 1, ACCT_TO" +
"    from excludeAccounts" +
"    where accounts + 1 <= ACCT_TO" +
")" +
" select accounts" +
" from  excludeAccounts" +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('"+transdate+ "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC in('ABD', 'OBD'))" +
" union" +
" (SELECT '3', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd " +
" where frvgtrd_GRNT_code = '" + GrantNumber + "'" +
" and   frvgtrd_ACCT_CODE  between '3300' and '3346' " +
" and frvgtrd_Acct_code not in " +
" ( " +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '3%' and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts " +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select accounts " +
" from  excludeAccounts " +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC in('ABD', 'OBD')) " +
" union" +
" (SELECT '4', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd" +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   (frvgtrd_ACCT_CODE  between '3800' and '3880' " +
"      or frvgtrd_ACCT_CODE between '6300' and '6330') " +
" and frvgtrd_Acct_code not in " +
" (" +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '3%' or FRRBECL_ACCT_CODE_FROM like '6%' and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts" +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select distinct accounts " +
" from  excludeAccounts " +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC in('ABD', 'OBD')) " +
" union " +
" (SELECT '5', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd" +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   (frvgtrd_ACCT_CODE  between '3010' and '3047' " +
"      or frvgtrd_ACCT_CODE between '4140' and '4140' " +
"      or frvgtrd_ACCT_CODE between '4201' and '4202' " +
"      )" +
" and frvgtrd_Acct_code not in " +
" (" +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '3%' or FRRBECL_ACCT_CODE_FROM like '4%' and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts " +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select distinct accounts " +
" from  excludeAccounts " +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC in('ABD', 'OBD')) " +
" union " +
" (SELECT '6', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd " +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   (frvgtrd_ACCT_CODE  between '3473' and '3473' " +
"      or frvgtrd_ACCT_CODE between '3528' and '3528' " +
"      or frvgtrd_ACCT_CODE between '3685' and '3685' " +
"      or frvgtrd_ACCT_CODE between '4514' and '4518' " +
"      )" +
" and frvgtrd_Acct_code not in " +
" (" +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '3%' or FRRBECL_ACCT_CODE_FROM like '4%' and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts " +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select distinct accounts" +
" from  excludeAccounts" +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC in('ABD', 'OBD')) " +
" union" +
" (SELECT '7', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd" +
" where frvgtrd_grnt_code = '" + GrantNumber + "' " +
" and   (frvgtrd_ACCT_CODE   between '3000' and '3001' " +
"      or frvgtrd_ACCT_CODE between '3050' and '3280' " +
"      or frvgtrd_ACCT_CODE between '3347' and '3472' " +
"      or frvgtrd_ACCT_CODE between '3474' and '3526' " +
"      or frvgtrd_ACCT_CODE between '3529' and '3680' " +
"      or frvgtrd_ACCT_CODE between '3696' and '3793' " +
"      or frvgtrd_ACCT_CODE between '3900' and '4135' " +
"      or frvgtrd_ACCT_CODE between '4145' and '4200' " +
"      or frvgtrd_ACCT_CODE between '4203' and '4500' " +
"      or frvgtrd_ACCT_CODE between '4540' and '4999' " +
"      or frvgtrd_ACCT_CODE between '6379' and '7000' " +
"      or frvgtrd_ACCT_CODE between '7002' and '8014' " +
"      )" +
" and frvgtrd_Acct_code not in " +
" (" +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO" +
"    from  frrbecl " +
"    where (FRRBECL_ACCT_CODE_FROM like '3%' or FRRBECL_ACCT_CODE_FROM like '4%' or FRRBECL_ACCT_CODE_FROM like '6%' or FRRBECL_ACCT_CODE_FROM like '8%') and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts " +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select distinct accounts " +
" from  excludeAccounts " +
" ) " +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC in('ABD', 'OBD')) " +
" union " +
" (SELECT '8', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd " +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   ( frvgtrd_ACCT_CODE  = '7001' ) " +
" and   trunc(frvgtrd_trans_date) <=  to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC in ('ABD', 'OBD')) ";
           

 		
           return CumeAmount;



       }






       public string CumeAmountYTD(string GrantNumber, string transdate)
       {
            string CumeAmount = @" SELECT '1', NVL(sum(frvgtrd_trans_amt),0) Total " +
"From frvgtrd f " +
"where frvgtrd_GRNT_code = '" + GrantNumber + "'" +
" and   frvgtrd_ACCT_CODE  between '2000' and '2899' " +
"and frvgtrd_Acct_code not in " +
"(" +
"with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '2%' and frrbecl_coas_code = 'D' " +
"    union all" +
"    select accounts + 1, ACCT_TO" +
"    from excludeAccounts" +
"    where accounts + 1 <= ACCT_TO" +
")" +
"select accounts " +
"from  excludeAccounts" +
")" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC = 'YTD' " +
" union" +
" (SELECT '2', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd" +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   frvgtrd_ACCT_CODE  between '2900' and '2999' " +
" and frvgtrd_Acct_code not in " +
"(" +
" with excludeAccounts (accounts, ACCT_TO) as ( " +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '2%' and frrbecl_coas_code = 'D' " +
"    union all" +
"    select accounts + 1, ACCT_TO" +
"    from excludeAccounts" +
"    where accounts + 1 <= ACCT_TO" +
")" +
" select accounts" +
" from  excludeAccounts" +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC = 'YTD')" +
" union" +
" (SELECT '3', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd " +
" where frvgtrd_GRNT_code = '" + GrantNumber + "'" +
" and   frvgtrd_ACCT_CODE  between '3300' and '3346' " +
" and frvgtrd_Acct_code not in " +
" ( " +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '3%' and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts " +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select accounts " +
" from  excludeAccounts " +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC = 'YTD') " +
" union" +
" (SELECT '4', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd" +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   (frvgtrd_ACCT_CODE  between '3800' and '3880' " +
"      or frvgtrd_ACCT_CODE between '6300' and '6330') " +
" and frvgtrd_Acct_code not in " +
" (" +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '3%' or FRRBECL_ACCT_CODE_FROM like '6%' and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts" +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select distinct accounts " +
" from  excludeAccounts " +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC = 'YTD') " +
" union " +
" (SELECT '5', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd" +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   (frvgtrd_ACCT_CODE  between '3010' and '3047' " +
"      or frvgtrd_ACCT_CODE between '4140' and '4140' " +
"      or frvgtrd_ACCT_CODE between '4201' and '4202' " +
"      )" +
" and frvgtrd_Acct_code not in " +
" (" +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '3%' or FRRBECL_ACCT_CODE_FROM like '4%' and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts " +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select distinct accounts " +
" from  excludeAccounts " +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC = 'YTD') " +
" union " +
" (SELECT '6', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd " +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   (frvgtrd_ACCT_CODE  between '3473' and '3473' " +
"      or frvgtrd_ACCT_CODE between '3528' and '3528' " +
"      or frvgtrd_ACCT_CODE between '3685' and '3685' " +
"      or frvgtrd_ACCT_CODE between '4514' and '4518' " +
"      )" +
" and frvgtrd_Acct_code not in " +
" (" +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO " +
"    from  frrbecl where FRRBECL_ACCT_CODE_FROM like '3%' or FRRBECL_ACCT_CODE_FROM like '4%' and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts " +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select distinct accounts" +
" from  excludeAccounts" +
" )" +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC = 'YTD') " +
" union" +
" (SELECT '7', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd" +
" where frvgtrd_grnt_code = '" + GrantNumber + "' " +
" and   (frvgtrd_ACCT_CODE   between '3000' and '3001' " +
"      or frvgtrd_ACCT_CODE between '3050' and '3280' " +
"      or frvgtrd_ACCT_CODE between '3347' and '3472' " +
"      or frvgtrd_ACCT_CODE between '3474' and '3526' " +
"      or frvgtrd_ACCT_CODE between '3529' and '3680' " +
"      or frvgtrd_ACCT_CODE between '3696' and '3793' " +
"      or frvgtrd_ACCT_CODE between '3900' and '4135' " +
"      or frvgtrd_ACCT_CODE between '4145' and '4200' " +
"      or frvgtrd_ACCT_CODE between '4203' and '4500' " +
"      or frvgtrd_ACCT_CODE between '4540' and '4999' " +
"      or frvgtrd_ACCT_CODE between '6379' and '7000' " +
"      or frvgtrd_ACCT_CODE between '7002' and '8014' " +
"      )" +
" and frvgtrd_Acct_code not in " +
" (" +
" with excludeAccounts (accounts, ACCT_TO) as (" +
"    select to_number(FRRBECL_ACCT_CODE_FROM) as accounts,  to_number(FRRBECL_ACCT_CODE_TO) as ACCT_TO" +
"    from  frrbecl " +
"    where (FRRBECL_ACCT_CODE_FROM like '3%' or FRRBECL_ACCT_CODE_FROM like '4%' or FRRBECL_ACCT_CODE_FROM like '6%' or FRRBECL_ACCT_CODE_FROM like '8%') and frrbecl_coas_code = 'D' " +
"    union all " +
"    select accounts + 1, ACCT_TO " +
"    from excludeAccounts " +
"    where accounts + 1 <= ACCT_TO " +
")" +
" select distinct accounts " +
" from  excludeAccounts " +
" ) " +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC = 'YTD') " +
" union " +
" (SELECT '8', NVL(sum(frvgtrd_trans_amt),0) " +
" From frvgtrd " +
" where frvgtrd_GRNT_code = '" + GrantNumber + "' " +
" and   ( frvgtrd_ACCT_CODE  = '7001' ) " +
" and   trunc(frvgtrd_trans_date) <= to_date('" + transdate + "', 'DD-MON-YY') " +
" and   FRVGTRD_FIELD_DESC = 'YTD') ";                                                                                                             

           return CumeAmount;



       }

       public string GetGrants()
       {
           string grants =
                        "with grantbalance as                                                                                  " +
            "(                                                                                                     " +
            "select tr.trraccd_grnt_code, nvl(sum(nvl(trraccd_balance,0)),0) prior_due from TRRACCD tr, ftvfund f  " +
            "where   FTVFUND_NCHG_DATE > sysdate                                                                   " +
            "and    FTVFUND_COAS_CODE in ( 'D','S')                                                                " +
            "AND    ftvfund_grnt_code =  tr.trraccd_grnt_code                                                      " +
            "And    ftvfund_grnt_code is not null                                                                  " +
            "group by tr.trraccd_grnt_code                                                                         " +
            ")" +
            "select distinct                                                                                            " +
            "       fr.frbgrnt_code,                                                                                    " +
            "       fr.FRBGRNT_LONG_TITLE,                                                                              " +
            "       sp.spriden_first_name ||' '|| sp.SPRIDEN_LAST_NAME  PI, " +
            "       sp.SPRIDEN_LAST_NAME  AS PI_LAST_NAME ,                                                             " +
            "       fr.FRBGRNT_PROJECT_START_DATE,                                                                      " +
            "       fr.FRBGRNT_PROJECT_END_DATE,                                                                        " +
            "       fr.FRBGRNT_SPONSOR_ID, fr.FRBGRNT_COAS_CODE,    " +
            "       bill.FRBGBIL_ADDR_TYPE,                                                                             " +
            "       bill.FRBGBIL_ADDR_SEQNO,                                                                            " +
            "       addr.SPRADDR_STREET_LINE1,                                                                          " +
            "       addr.spraddr_street_line2,                                                                          " +
            "       addr.spraddr_street_line3,                                                                          " +
            "       addr.spraddr_street_line4,                                                                          " +
            "       addr.SPRADDR_CITY,                                                                                  " +
            "       addr.SPRADDR_STAT_CODE,                                                                             " +
            "       addr.spraddr_zip,                                                                                   " +
            "       addr.SPRADDR_NATN_CODE,                                                                             " +
            "       event.FRBEVNG_BFRM_CODE,                                                                            " +
            "       FRRGRPI_ID_PIDM,                                                                                    " +
            "       FRRGRPI_ID_IND  , ag.SPRIDEN_LAST_NAME AGENCY, FRBGRNT_SPONSOR_ID , nvl(FRBGRNT_MAX_FUNDING_AMT,0) award_amount ," +
            "        nvl(prior_due,0)  prior_due ,   FRBGRNT_ALTERNATE_STATUS_DESC " +
            "from   frbgrnt fr, ftvfund fund, spriden sp, frbgbil bill, spraddr addr, frbevng event,FRRGRPI, spriden ag, grantbalance " +
            "where  fr.frbgrnt_code = fund.FTVFUND_GRNT_CODE                                                            " +
            "and    fund.FTVFUND_NCHG_DATE > sysdate                                                                    " +
            "and    fund.FTVFUND_COAS_CODE in ( 'D','S')                                                                " +
            "and    sp.spriden_change_ind is null                                                                       " +
            "and    sp.spriden_pidm = fr.FRBGRNT_PI_PIDM                                                                " +
            "and    bill.FRBGBIL_GRNT_CODE = frbgrnt_code                                                               " +
            "and    addr.SPRADDR_ATYP_CODE = bill.FRBGBIL_ADDR_TYPE                                                     " +
            "and    addr.SPRADDR_SEQNO = bill.FRBGBIL_ADDR_SEQNO                                                        " +
            "AND    addr.SPRADDR_PIDM = fr.FRBGRNT_AGENCY_PIDM                                                          " +
            "and    event.FRBEVNG_GRNT_CODE(+) = fr.FRBGRNT_CODE                                                        " +
            "and    FRRGRPI_ID_IND(+) in ( '002','005')                                                                 " +
            "and    FRRGRPI_GRNT_CODE(+) = fr.FRBGRNT_CODE                                                               " +
            "and    fr.frbgrnt_agency_pidm(+) = ag.spriden_pidm " +
            "and    ag.spriden_change_ind is null            " +
            "and    fr.frbgrnt_code = trraccd_grnt_code(+)";
            
             return grants;
       }

       public string GetGrantAccounts()
       {
        
        string accountants =
        "select distinct  " +
        "S1.spriden_last_name Last_Name,            " +
        "S1.SPRIDEN_FIRST_NAME First_name,          " +
        "FRRGRPI_ID_IND,                            " +
        "FRRGRPI_ID_PIDM ,   S2.SPRIDEN_ID USERID  , GOREMAL_EMAIL_ADDRESS , T.SPRTELE_PHONE_AREA || '-'|| substr(SPRTELE_PHONE_NUMBER,1,3)||'-'||substr(sprtele_phone_number,4,4) AS PHONE   " +
        "from FRRGRPI, SPRIDEN S1, SPRIDEN S2  , GOREMAL  G  , SPRTELE T             " +
        "where FRRGRPI_ID_IND in ( '002','005')          " +
        "AND S1.spriden_PIDM = FRRGRPI_ID_PIDM              " +
        "AND S2.spriden_pidm =  FRRGRPI_ID_PIDM " +
        "AND S1.spriden_change_ind is null  AND  S2.SPRIDEN_USER = 'USERID1'"
        + " AND G.GOREMAL_PIDM =  FRRGRPI_ID_PIDM      AND GOREMAL_EMAL_CODE(+) = 'DOE' "
        +"AND T.SPRTELE_TELE_CODE(+) = 'CA' "
        +"AND T.SPRTELE_STATUS_IND is null "
        +"and T.SPRTELE_PRIMARY_IND(+) = 'Y' "
        +"AND T.SPRTELE_PIDM = FRRGRPI_ID_PIDM(+)   ";       
        return accountants;

       }
       


       




       private  string getAccount()
       {
        string acctCommandString =
        " select ftvacct_coas_code, " +
        " ftvacct_acct_code, "+
        " ftvacct_data_entry_ind, "+
        " ftvacct_status_ind, " +
        " ftvacct_title " +
        " from FTVACCT  "+
        " WHERE trunc(ftvacct_eff_date) <= trunc(SYSDATE) "+
        " and trunc(ftvacct_nchg_date) > trunc(sysdate)  "+
        " AND ftvacct_coas_code IN ('D','S') "+
        " and (ftvacct_term_date is null or " +
                       "ftvacct_term_date > trunc(sysdate)) "; 
          
           return acctCommandString;

       }
       
       private string getFunds()
       {
           string fundCmdString = "select FTVFUND_FUND_CODE," +
                   "FTVFUND_ORGN_CODE_DEF, " +
                   "FTVFUND_PROG_CODE_DEF, " +
                   "FTVFUND_COAS_CODE, "+
                   "FTVFUND_TITLE " +
                   "from FTVFUND  " +
                   "where FTVFUND_COAS_CODE IN ( 'D','S') " +
                    "and FTVFUND_EFF_DATE <= sysdate " +
                    "and FTVFUND_NCHG_DATE > sysdate " +
                    "and ((FTVFUND_TERM_DATE is null or FTVFUND_TERM_DATE > (sysdate)) " +
                           "OR (ftvfund_term_date <= (sysdate) " +
                               "and FTVFUND_EXPEND_END_DATE is not NULL " +
                               "and FTVFUND_EXPEND_END_DATE >= (sysdate))) " +
                     "and FTVFUND_STATUS_IND = 'A' " +
                     "and FTVFUND_DATA_ENTRY_IND = 'Y' ";
           
           return fundCmdString;
       }


       private string getOrgs()
       {
           string OrgCmdString =
             " select distinct "         +
             " FTVORGN_COAS_CODE, "      +
             " FTVORGN_ORGN_CODE, "      +
             " FTVORGN_DATA_ENTRY_IND, " +
             " FTVORGN_TITLE, "          +
             " FTVORGN_PROG_CODE_DEF "   +
             " from FTVORGN "            +
             "		where FTVORGN_COAS_CODE in('S','D') " +
             "		  and FTVORGN_EFF_DATE <= SYSDATE "   +
             "		  and (FTVORGN_NCHG_DATE is NULL or FTVORGN_NCHG_DATE > SYSDATE) " +
             "		  and (FTVORGN_TERM_DATE is null or FTVORGN_TERM_DATE > SYSDATE) " +
             "		  and FTVORGN_STATUS_IND = 'A' " +
             "		  and FTVORGN_DATA_ENTRY_IND = 'Y' ";

           return OrgCmdString;

       }

       private string getLocations()
       {
           string LocCmdString =
       "select FTVLOCN_COAS_CODE," +
       "FTVLOCN_LOCN_CODE," +
       "FTVLOCN_TITLE," +
       "FTVLOCN_STATUS_IND " +
            " from FTVLOCN " +
            "where FTVLOCN_COAS_CODE in ('S','D') " +
              " and FTVLOCN_EFF_DATE <= (sysdate) " +
              " and (FTVLOCN_NCHG_DATE is NULL or FTVLOCN_NCHG_DATE > sysdate) " +
              " and (FTVLOCN_TERM_DATE is null or FTVLOCN_TERM_DATE > sysdate) " +
              " and FTVLOCN_STATUS_IND = 'A' ";

           return LocCmdString;
       }
       private string getPrograms()
       {
        string progCmdString =
            "select ftvprog_coas_code," +
           "ftvprog_prog_Code," +
           "ftvprog_title " +
                " from FTVPROG " +
                "where FTVPROG_COAS_CODE in ('D','S') " +
                  " and FTVPROG_EFF_DATE <= sysdate " +
                  " and (FTVPROG_NCHG_DATE is NULL or FTVPROG_NCHG_DATE > sysdate) " +
                  " and (FTVPROG_TERM_DATE is null or FTVPROG_TERM_DATE > sysdate) " +
                  " and FTVPROG_STATUS_IND = 'A' " +
                  " and FTVPROG_DATA_ENTRY_IND = 'Y' ";
        return progCmdString;
       }

       private string getActivity()
       {

           var activityCmdString =
               "select ftvactv_coas_code ," +
                      "ftvactv_actv_Code," +
                      "FTVACTV_STATUS_IND, " +
                      "FTVACTV_TITLE " +
            " from FTVACTV " +
            "where FTVACTV_COAS_CODE in('D','S') " +
              " and FTVACTV_EFF_DATE <= sysdate" +
              " and (FTVACTV_NCHG_DATE is NULL or FTVACTV_NCHG_DATE > sysdate) " +
              " and (FTVACTV_TERM_DATE is null or FTVACTV_TERM_DATE > sysdate) " +
              " and FTVACTV_STATUS_IND = 'A'";
           return activityCmdString;
       }

       private string getApprovers()
       {
           var approverString =
               " select distinct fzrccus_coas_code, " +
                      " decode(fzrccus_fund_code, " +
                             " 'DFT', " +
                             " dft_fo.a," +
                             " fzrccus_fund_code) FUND, " +
                      " fzrccus_orgn_code, " +
                      " fzrccus_ccrc_code, " +
                      " spriden_id, " +
                      " spriden_last_name, " +
                      " spriden_first_name, " +
                      " goremal.GOREMAL_EMAIL_ADDRESS " +
        " from fzrccus, " +
             " (select distinct ftvfund_fund_code a, " +
                              " ftvfund_orgn_code_def b," +
                              " 'DFT' c " +
                " from ftvfund " +
               "  where ftvfund_orgn_code_def in " +
                     " (select distinct fzrccus_orgn_code " +
                        " from fzrccus " +
                       "  where fzrccus_fund_code = 'DFT' " +
                       "  and fzrccus_ccrc_code in ('SSR100', 'SSR200') " +
                       "  and fzrccus_coas_code in ('D', 'S')) " +
                 " and to_char(ftvfund_nchg_date, 'DD-MON-YYYY') = " +
                 "    '31-DEC-2099' " +
                " and ftvfund_status_ind = 'A' " +
                " and ftvfund_data_entry_ind = 'Y' " +
                " and ftvfund_coas_code in ('D', 'S') " +
                " and (ftvfund_term_date > sysdate or " +
                "     ftvfund_term_date is null) " +
                " and ftvfund_ftyp_code not in ('31', '41', '51', 'BK')) dft_fo, " +
            " spriden, goremal " +
       " where fzrccus_ccrc_code in ('SSR100', 'SSR200') " +
       "  and fzrccus_coas_code in ('D', 'S') " +
       "  and fzrccus_fund_code = dft_fo.c(+) " +
       "  and fzrccus_orgn_code = dft_fo.b(+) " +
       "  and decode(fzrccus_fund_code, 'DFT', dft_fo.a, fzrccus_fund_code) is not null " +
       "  and spriden_pidm = fzrccus_pidm " +
       "  and spriden_user = 'USERID1' " +
       "  and goremal_pidm(+)= fzrccus_pidm " +
       "  and goremal_emal_code(+) = 'DOE' " +
       "  order by fzrccus_ccrc_code, " +
               " fzrccus_coas_code, " +
               " decode(fzrccus_fund_code, " +
               "        'DFT', " +
               "        dft_fo.a, " +
               "        fzrccus_fund_code), " +
               " fzrccus_orgn_code, " +
               " spriden_id";

           return approverString;
       }


       private string GetEmployeesInformation()
       {
           var employeeInfo = "SELECT " +
                "upper(B.PZEBORGP_PRIMARY_USERID)   AS EMPLOYEE_USER_ID, " +
                "B.PZEBORGP_FIRST_NAME                  AS EMPLOYEE_FIRST_NAME,           " +
                "B.PZEBORGP_LAST_NAME                   AS EMPLOYEE_LAST_NAME ,           " +
                "B.PZEBORGP_POSN                        AS EMPLOYEE_POSITION_NUMBER,      " +
                "B.PZEBORGP_EMAIL_ADDRESS               AS EMPLOYEE_EMAIL_ADDRESS,        " +
                "B.PZEBORGP_SUPERVISOR_POSN             AS SUPERVISOR_POSITION_NUMBER,    " +
                "A.PZEBORGP_FIRST_NAME                  AS SUPERVISOR_FIRST_NAME,         " +
                "A.PZEBORGP_LAST_NAME                   AS SUPERVISOR_LAST_NAME ,         " +
                "A.PZEBORGP_PRIMARY_USERID   AS SUPERVISOR_USER_ID,                       " +
                "A.PZEBORGP_EMAIL_ADDRESS    AS SUPERVISOR_EMAIL_ADDRESS                  " +
                "FROM                                                                     " +
                "(select                                                                  " +
                "PZEBORGP_FIRST_NAME,                                                     " +
                "PZEBORGP_LAST_NAME ,                                                     " +
                "PZEBORGP_POSN,                                                           " +
                "PZEBORGP_SUPERVISOR_POSN,                                                " +
                "PZEBORGP_PRIMARY_USERID,                                                 " +
                "PZEBORGP_EMAIL_ADDRESS                                                   " +
                "from PZEBORGP)A,                                                         " +
                "(SELECT                                                                  " +
                "PZEBORGP_FIRST_NAME,                                                     " +
                "PZEBORGP_LAST_NAME ,                                                     " +
                "PZEBORGP_POSN,                                                           " +
                "PZEBORGP_SUPERVISOR_POSN,                                                " +
                "PZEBORGP_PRIMARY_USERID,                                                 " +
                "PZEBORGP_EMAIL_ADDRESS                                                   " +
                "from PZEBORGP)B                                                          " +
                "WHERE A.PZEBORGP_POSN  = B.PZEBORGP_SUPERVISOR_POSN                      ";


               return employeeInfo;



       }


       public string GetMyCompletedJournals(String theListOfJournalPendingBanner)
       {

            var completeJournals="select distinct FGBTRNH_DOC_CODE "+ 
                                  "FROM  fgbtrnh " +
                                  "WHERE fgbtrnh_doc_code " +
                                  "IN ("+ theListOfJournalPendingBanner + ")";

            return completeJournals;

       }



       public string GetCurrentAmount(string grant, string invoice, string adjustmentNo, string formatCode)
       {
           string currentBillAmount = @" SELECT '2',FRRGENB_GRNT_CODE , NVL(SUM(nvl(FRRGENB_BILL_AMT,0)),0) AS  TOTAL " +
" from frrbfrm, frrgenb " +
"where Frrgenb_grnt_code = '"+grant+"' " +
"and frrbfrm_code = '" + formatCode + "' " +
"and FRRGENB_BILL_INV_SEQ_NO = '" + invoice + "' " +
"and FRRGENB_BILL_INV_ADJ_NO = '" + adjustmentNo + "'" +
"and FRRBFRM_GROUP_SEQ_NO = '2' " +
"and FRRGENB_GROUP_SEQ_NO = FRRBFRM_GROUP_SEQ_NO " +
"and FRRGENB_COMPLETE_IND = 'Y' " +
" GROUP BY FRRGENB_GRNT_CODE " +
"union" +
"(SELECT '1',FRRGENB_GRNT_CODE , nvl(sum(nvl(FRRGENB_BILL_AMT,0)),0) AS \"Salary Current\" " +
" from frrbfrm, frrgenb " +
" where Frrgenb_grnt_code = '" + grant + "' " +
"and frrbfrm_code = '" + formatCode + "' " +
"and FRRGENB_BILL_INV_SEQ_NO ='" + invoice + "' " +
"and FRRGENB_BILL_INV_ADJ_NO = '" + adjustmentNo + "'" +
"and FRRBFRM_GROUP_SEQ_NO = '1' " +
"and FRRGENB_GROUP_SEQ_NO = FRRBFRM_GROUP_SEQ_NO " +
"and FRRGENB_COMPLETE_IND = 'Y' " +
" GROUP BY FRRGENB_GRNT_CODE " +
")  " +
"union" +
"(SELECT '3',FRRGENB_GRNT_CODE , nvl(sum(nvl(FRRGENB_BILL_AMT,0)),0) \"Consultant_Current\"" +
"from frrbfrm, frrgenb " +
" where Frrgenb_grnt_code = '" + grant + "' " +
"and frrbfrm_code = '" + formatCode + "' " +
"and FRRGENB_BILL_INV_SEQ_NO ='" + invoice + "' " +
"and FRRGENB_BILL_INV_ADJ_NO = '" + adjustmentNo + "'" +
"and FRRBFRM_GROUP_SEQ_NO = '3'" +
"and FRRGENB_GROUP_SEQ_NO = FRRBFRM_GROUP_SEQ_NO " +
"and FRRGENB_COMPLETE_IND = 'Y' " +
" GROUP BY FRRGENB_GRNT_CODE )" +
"UNION" +
"(" +
"SELECT '5' ,FRRGENB_GRNT_CODE , nvl(sum(nvl(FRRGENB_BILL_AMT,0)),0)" +
" from frrbfrm, frrgenb " +
" where Frrgenb_grnt_code = '" + grant + "' " +
"and frrbfrm_code = '" + formatCode + "' " +
"and FRRGENB_BILL_INV_SEQ_NO ='" + invoice + "' " +
"and FRRGENB_BILL_INV_ADJ_NO = '" + adjustmentNo + "'" +
"and FRRBFRM_GROUP_SEQ_NO = '5'" +
"and FRRGENB_GROUP_SEQ_NO = FRRBFRM_GROUP_SEQ_NO " +
"and FRRGENB_COMPLETE_IND = 'Y' " +
" group  by FRRGENB_GRNT_CODE" +
")" +
"UNION" +
"(" +
"SELECT '6' , FRRGENB_GRNT_CODE , nvl(sum(nvl(FRRGENB_BILL_AMT,0)),0)" +
" FROM frrbfrm, frrgenb " +
" where Frrgenb_grnt_code = '" + grant + "' " +
"and frrbfrm_code = '" + formatCode + "' " +
"and FRRGENB_BILL_INV_SEQ_NO ='" + invoice + "' " +
"and FRRGENB_BILL_INV_ADJ_NO = '" + adjustmentNo + "'" +
"and FRRBFRM_GROUP_SEQ_NO = '6'" +
"and FRRGENB_GROUP_SEQ_NO = FRRBFRM_GROUP_SEQ_NO " +
"and FRRGENB_COMPLETE_IND = 'Y' " +
" GROUP BY FRRGENB_GRNT_CODE)" +
"UNION" +
"(SELECT '7' , FRRGENB_GRNT_CODE , nvl(sum(nvl(FRRGENB_BILL_AMT,0)),0)" +
" from frrbfrm, frrgenb " +
" where Frrgenb_grnt_code = '" + grant + "' " +
"and frrbfrm_code = '" + formatCode + "' " +
"and FRRGENB_BILL_INV_SEQ_NO ='" + invoice + "' " +
"and FRRGENB_BILL_INV_ADJ_NO = '" + adjustmentNo + "'" +
"and FRRBFRM_GROUP_SEQ_NO = '7'" +
"and FRRGENB_GROUP_SEQ_NO = FRRBFRM_GROUP_SEQ_NO " +
"and FRRGENB_COMPLETE_IND = 'Y' " +
" group by FRRGENB_GRNT_CODE )" +
"UNION" +
"(SELECT '8' , FRRGENB_GRNT_CODE , nvl(sum(nvl(FRRGENB_BILL_AMT,0)),0)" +
" from frrbfrm, frrgenb " +
" where Frrgenb_grnt_code = '" + grant + "' " +
"and frrbfrm_code = '" + formatCode + "' " +
"and FRRGENB_BILL_INV_SEQ_NO ='" + invoice + "' " +
"and FRRGENB_BILL_INV_ADJ_NO = '" + adjustmentNo + "'" +
"and FRRBFRM_GROUP_SEQ_NO = '8'" +
"and FRRGENB_GROUP_SEQ_NO = FRRBFRM_GROUP_SEQ_NO " +
"and FRRGENB_COMPLETE_IND = 'Y' " +
" group by FRRGENB_GRNT_CODE" +
")" +
"UNION" +
"(SELECT '4' , FRRGENB_GRNT_CODE , nvl(sum(nvl(FRRGENB_BILL_AMT,0)),0)" +
" from frrbfrm, frrgenb " +
" where Frrgenb_grnt_code = '" + grant + "' " +
"and frrbfrm_code = '" + formatCode + "' " +
"and FRRGENB_BILL_INV_SEQ_NO ='" + invoice + "' " +
"and FRRGENB_BILL_INV_ADJ_NO = '" + adjustmentNo + "'" +
"and FRRBFRM_GROUP_SEQ_NO = '4'" +
"and FRRGENB_GROUP_SEQ_NO = FRRBFRM_GROUP_SEQ_NO " +
"and FRRGENB_COMPLETE_IND = 'Y' " +
" group by FRRGENB_GRNT_CODE" +
")";


           return currentBillAmount;

       }


    }
}
