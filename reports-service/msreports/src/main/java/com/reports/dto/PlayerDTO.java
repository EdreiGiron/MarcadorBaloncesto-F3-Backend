package com.reports.dto;


import lombok.*;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class PlayerDTO {
    private int id;
    private int teamId;
    private int number;
    private String fullName;
    private String position;
    private String nationality;
}
