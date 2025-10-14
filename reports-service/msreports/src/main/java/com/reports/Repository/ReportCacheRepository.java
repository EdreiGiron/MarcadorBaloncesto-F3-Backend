package com.reports.Repository;

import com.reports.Entity.ReportCache;
import org.springframework.data.jpa.repository.JpaRepository;
import java.util.Optional;

public interface ReportCacheRepository extends JpaRepository<ReportCache, Long> {
    Optional<ReportCache> findByKey(String key);
}
