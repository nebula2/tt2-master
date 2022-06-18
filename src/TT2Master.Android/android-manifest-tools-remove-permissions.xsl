<xsl:stylesheet 
	version="1.0" 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:tools="http://schemas.android.com/tools" 
	xmlns:android="http://schemas.android.com/apk/res/android">
	
<xsl:output method="xml" indent="yes"/>
<xsl:strip-space elements="*" />

<xsl:variable name="removed-permissions" select=".//uses-permission[@tools:node='remove']" />

<xsl:template match="@*|node()">
 <xsl:copy>
  <xsl:apply-templates select="@*|node()"/>
 </xsl:copy>
</xsl:template>

<xsl:template match="uses-permission">
	<xsl:variable name="name" select="@android:name" />
	
	<xsl:if test="count($removed-permissions[@android:name = $name]) = 0">
		<xsl:copy-of select="." />
	</xsl:if>
</xsl:template>

</xsl:stylesheet>