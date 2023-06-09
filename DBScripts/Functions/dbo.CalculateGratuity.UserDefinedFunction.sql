SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [CalculateGratuity](
     @Emp nvarchar(2), 
	 @Curr_Date Date
)
RETURNS decimal(15,3)
AS
BEGIN

  DECLARE @v_Doa				Date;
  DECLARE @v_First_Year_Leave	int;
  DECLARE @Total_Years			int;
  DECLARE @Total_Days			int;
  DECLARE @Total_Months			int;
  DECLARE @Cur_Year_Start		Date;
  DECLARE @Curr_Basic			decimal(15, 3);
  DECLARE @Gratuity				decimal(15, 3);
  DECLARE @Nationality			int;

 ------------- Start to Find Total Joining Days of Particular Employee from Employee Main Table

  Select  @v_Doa = DOA, @Total_Years = (DATEDIFF(dy, DOA, @Curr_Date) + 1) /365, @Nationality = Nationality,@Curr_Basic = BasicSalary
   From  HR_Employee_s
   Where [EmployeeNumber] = @Emp;

   ------------- End to Find Total Joining Days of Particular Employee from Employee Main Table
  If @Nationality = 1 
   BEGIN	SET @Gratuity = 0; END
  Else
   BEGIN
	 If @Total_Years >= 1
		BEGIN
		 If @Total_Years < =3
			BEGIN
				SET @Gratuity = (@Curr_Basic / 2) * @Total_Years;
				SET @Cur_Year_Start = DateAdd(month,@Total_Years * 12, @v_Doa);

				SET @Total_Months = DATEDIFF(month,  @Cur_Year_Start,@Curr_Date);
                                                      
				SET @Gratuity   = @Gratuity + ((@Curr_Basic / 2) / 12 * @Total_Months);
				SET @Total_Days = DATEDIFF(dd, DateAdd(month,@Total_Months,@Cur_Year_Start),@Curr_Date);
                                                       
				SET @Gratuity  = @Gratuity + ((@Curr_Basic / 2) / 365 * @Total_Days);
			END
		  Else
            BEGIN
		        SET @Gratuity = @Curr_Basic * 1.5;
				SET @Gratuity = @Gratuity + (@Curr_Basic * (@Total_Years - 3));

				SET @Cur_Year_Start = DateAdd(month,@Total_Years * 12, @v_Doa);
															   --    
				SET @Total_Months = DATEDIFF(month, @Cur_Year_Start,@Curr_Date);
                                                     
				SET @Gratuity = @Gratuity + ((@Curr_Basic / 12) * @Total_Months);
                                                        
				SET @Total_Days = DATEDIFF(dd, DateAdd(month,@Total_Months,@Cur_Year_Start),@Curr_Date);

				SET @Gratuity = @Gratuity + ((@Curr_Basic / 365) * @Total_Days);
			END                                     
		END
    Else
  	  BEGIN	
		SET @Gratuity = 0; 
	   END
    END

	
	RETURN @Gratuity;
    
END;


GO
