using Entities;
using DTOs;

namespace Mappers;

public record LogListDTO(
    List<LogEntryDTO> log
);

public record LogEntryDTO(
    string type,
    string wallet_id,
    string stock_name
);

public static class AuditMapper
{
    public static LogListDTO MapToLogListDTO(List<AuditLog> auditLogs)
    {
        return new LogListDTO(
            log: auditLogs.Select(log => new LogEntryDTO(
                type: log.Type.ToString().ToLower(),

                wallet_id: log.UsedWallet.Id.ToString(),

                stock_name: log.UsedStock.Name
            )).ToList()
        );
    }
}