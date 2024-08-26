using BotKernel.Data;
using BotKernel.Extensions;
using BotKernel.Models;
using Dapper;

namespace BotKernel.Repositories
{
    public class EmployeeContactsRepository
    {
        private readonly OracleDbContext _context;

        public EmployeeContactsRepository(OracleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserProfile>> GetUserProfileListByKeywordAsync(string keyword)
        {
            keyword = keyword.Trim().ToUpper();

            using var connection = _context.CreateConnection();

            var query = @"
                WITH ID_LIST AS (
                    SELECT IDNO FROM VHPNPULB WHERE CNM LIKE '%' || :keyword || '%'
                    UNION
                    SELECT IDNO FROM RPUBTEL WHERE TEL LIKE :keyword || '%'
                    UNION
                    SELECT IDNO FROM RPUBTEL WHERE CHTNO LIKE :keyword || '%'
                    UNION
                    SELECT IDNO FROM RPUBEMAIL WHERE UPPER(EMAIL) LIKE :keyword || '%'
                    UNION
                    SELECT SNTIDNO AS IDNO FROM VSYSSNT0 WHERE TRIM(UPPER(SNTOPID)) = :keyword
                )
                SELECT
                    IDNO AS Id,
                    RAWTOHEX(A.CNM) AS Name,
                    ORGNID AS OrganizationId,
                    DPID AS DepartmentId,
                    C.CNM AS DepartmentName,
                    DUTID AS TitleId,
                    FHSLGDUT(DUTID, ORGNID) AS TitleName,
                    KHPNFC00.GETMAYR(IDNO, INBUSDAT, INDAT, OUTDAT) AS YearsOfExperience
                FROM
                    VHPNPULB A
                JOIN
                    ID_LIST B USING (IDNO)
                JOIN
                    RASBDEPT C USING (ORGNID, DPID)
                WHERE
                    OUTDAT IS NULL AND
                    ORGNID IN ('G', 'K', 'V')
            ";

            var profileList = await connection.QueryAsync<UserProfile>(query, new { keyword });

            foreach (var profile in profileList)
            {
                profile.Name = profile.Name.ToUtf8();
            }

            return profileList;
        }

        public async Task<UserProfile?> GetUserProfileByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();

            var query = @"
                SELECT
                    IDNO AS Id,
                    RAWTOHEX(A.CNM) AS Name,
                    ORGNID AS OrganizationId,
                    DPID AS DepartmentId,
                    B.CNM AS DepartmentName,
                    DUTID AS TitleId,
                    FHSLGDUT(DUTID, ORGNID) AS TitleName,
                    KHPNFC00.GETMAYR(IDNO, INBUSDAT, INDAT, OUTDAT) AS YearsOfExperience
                FROM
                    VHPNPULB A
                JOIN
                    RASBDEPT B USING (ORGNID, DPID)
                WHERE
                    OUTDAT IS NULL AND
                    ORGNID IN ('G', 'K', 'V') AND
                    IDNO = :id
            ";

            var profile = await connection.QueryFirstOrDefaultAsync<UserProfile>(query, new { id });

            if (profile != null)
            {
                profile.Name = profile.Name.ToUtf8();

            }

            return profile;
        }

        public async Task<UserContact?> GetContactByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();

            var query = @"
                SELECT
                    TEL AS PhoneNumber,
                    CHTNO AS MobileNumber,
                    EMAIL AS Email
                FROM
                    RPUBTEL
                FULL JOIN
                    RPUBEMAIL USING (IDNO)
                WHERE
                    IDNO = :id
            ";

            return await connection.QueryFirstOrDefaultAsync<UserContact>(query, new { id });
        }
    }
}
